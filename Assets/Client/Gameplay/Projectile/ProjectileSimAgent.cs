using Client.Gameplay.Health;
using Client.Gameplay.Map;
using Client.Gameplay.Movement;
using Client.Gameplay.Projectile.Network;
using UnityEngine;

namespace Client.Gameplay.Projectile
{
    public class ProjectileSimAgent : MonoBehaviour, IChunkSimEntity, IStateProvider<ProjectileState>
    {
        [SerializeField] private SimpleRider   _rider;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private float         _autoDestroyDelay = 3f;

        private Transform                _tr;
        private Transform                _target;
        private Vector2                  _initDirection;
        private GameplayContextBehaviour _gameplayContext;
        private int                      _damage;
        private float                    _leftToDestroy;

        public uint Id { get; private set; }
        public bool IsActive => gameObject.activeInHierarchy;
        public Vector3 Position => transform.position;
        public ProjectileContext ProjectileContext { get; private set; }


        private void Awake()
        {
            _tr = transform;
            _gameplayContext = GameplayContextBehaviour.Instance;
            _leftToDestroy = _autoDestroyDelay;
        }

        public void Init(in ProjectileSpawnData data, ProjectileContext context)
        {
            Id = data.Id;
            ProjectileContext = context;
            _damage = data.Stats.Damage;

            _rider.ChangeSpeed(data.Stats.Speed, data.Stats.MaxSpeed);

            Transform target = default;
            if (_gameplayContext.NpcSpawner.TryGetSpawned(data.TargetId, out var npc))
            {
                target = npc.transform;
                _initDirection = CalculateDirection(target);
            }
            else
            {
                _initDirection = transform.forward;
                return;
            }

            switch (data.Stats.TypeId)
            {
                case 0:
                    _trailRenderer.enabled = false;
                    _target = null;
                    break;

                case 1:
                    _trailRenderer.enabled = true;
                    _target = target;
                    break;
            }
        }

        public void Deactivate()
        {
            _target = null;
            _trailRenderer.Clear();
            _initDirection = Vector2.zero;
            _leftToDestroy = _autoDestroyDelay;
            _rider.SimulateStep(_initDirection, 1f);
            _rider.Reset();
        }

        public void Simulate(float delta)
        {
            var direction = _initDirection;
            if (_target != null)
            {
                direction = CalculateDirection(_target);
            }

            _rider.SimulateStep(direction, delta);

            _leftToDestroy -= delta;
            if (_leftToDestroy <= 0)
            {
                _gameplayContext.ProjectileSpawner.DespawnProjectile(Id);
            }
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

        private Vector2 CalculateDirection(Transform target)
        {
            var toTarget = target.position - _tr.position;
            var flat = new Vector2(toTarget.x, toTarget.z);
            var distance = flat.magnitude;

            // Preventing division by zero
            return flat / (distance > 1e-4f ? distance : 1f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.transform.TryGetComponent<EnemyHealthController>(out var healthController))
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