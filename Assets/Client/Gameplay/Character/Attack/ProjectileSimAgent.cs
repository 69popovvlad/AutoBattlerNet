using Client.Gameplay.Character.Attack.Network;
using Client.Gameplay.Health;
using Client.Gameplay.Map;
using Client.Gameplay.Movement;
using UnityEngine;

namespace Client.Gameplay.Character.Attack
{
    public class ProjectileSimAgent : MonoBehaviour, IChunkSimEntity, IStateProvider<ProjectileState>
    {
        [SerializeField] private SimpleRider _rider;
        [SerializeField] private float _arriveStopDist = 0.02f;
        [SerializeField] private TrailRenderer _trailRenderer;

        private Transform _tr;
        private Transform _target;
        private Vector3 _initDirection;
        private GameplayContextBehaviour _gameplayContext;
        private int _damage;

        public uint Id { get; private set; }
        public bool IsActive => gameObject.activeInHierarchy;
        public Vector3 Position => transform.position;
        public ProjectileContext ProjectileContext { get; private set; }


        private void Awake()
        {
            _tr = transform;
            _gameplayContext = GameplayContextBehaviour.Instance;
        }

        public void Init(in ProjectileSpawnData data, ProjectileContext context)
        {
            Id = data.Id;
            ProjectileContext = context;

            _initDirection = new Vector3(data.Direction.x, 0, data.Direction.y).normalized;
            _damage = data.Stats.Damage;

            switch (data.Stats.TypeId)
            {
                case 0:
                    _trailRenderer.enabled = false;
                    break;

                case 1:
                    _trailRenderer.enabled = true;

                    if (_gameplayContext.NpcNetClient.TryGetGhost(data.TargetId, out var characterContext))
                    {
                        _target = characterContext.transform;
                    }

                    break;
            }
        }

        public void Deactivate()
        {
            _target = null;
            _trailRenderer.Clear();
        }

        public void Simulate(float delta)
        {
            var direction = _initDirection;
            if (_target != null)
            {
                var toTarget = _target.position - _tr.position;
                var flat = new Vector2(toTarget.x, toTarget.z);
                var distance = flat.magnitude;

                if (distance > _arriveStopDist)
                {
                    // Preventing division by zero
                    direction = flat / (distance > 1e-4f ? distance : 1f);
                }
            }

            _rider.ApplyInputStep(direction, delta);
        }

        public ProjectileState ExtractState(uint tick)
        {
            var ks = _rider.GetState();
            return new ProjectileState
            {
                Tick = tick,
                Id = Id,
                Position = ks.Position,
                Velocity = ks.Velocity,
                Yaw = ks.Yaw
            };
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.transform.TryGetComponent<HealthController>(out var healthController))
            {
                return;
            }

            healthController.Damage(_damage);
            if (healthController.IsDead)
            {
                _gameplayContext.NpcSpawner.DespawnNpc(healthController.Id);
            }
            _gameplayContext.ProjectileSpawner.DespawnProjectile(Id);
        }
    }
}