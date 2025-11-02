using Client.Gameplay.Character.Attack.Network;
using Client.Gameplay.Map;
using Client.Gameplay.Movement;
using Client.Gameplay.Npc.Network;
using UnityEngine;

namespace Client.Gameplay.Character.Attack
{
    public class ProjectileSimAgent : MonoBehaviour, IChunkSimEntity, IStateProvider<ProjectileState>
    {
        [SerializeField] private SimpleRider _rider;
        [SerializeField] private float _arriveStopDist = 0.3f;
        [SerializeField] private TrailRenderer _trailRenderer;

        public uint Id { get; private set; }
        public bool IsActive => gameObject.activeInHierarchy;
        public Vector3 Position => transform.position;

        private Transform _tr;
        private Transform _target;
        private Vector3 _initDirection;
        private NpcNetClient _npcNetClient;

        private void Awake()
        {
            _tr = transform;
            _npcNetClient = GameplayContextBehaviour.Instance.NpcNetClient;
        }

        public void Init(in ProjectileSpawnData data)
        {
            Id = data.Id;
            _initDirection = new Vector3(data.Direction.x, 0, data.Direction.y);

            switch (data.Stats.TypeId)
            {
                case 0:
                    _trailRenderer.enabled = false;
                    break;

                case 1:
                    _trailRenderer.enabled = true;

                    if (_npcNetClient.TryGetGhost(data.TargetId, out var characterContext))
                    {
                        _target = characterContext.transform;
                    }

                    break;
            }
        }

        public void Deactivate()
        {
            _target = null;
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
    }
}