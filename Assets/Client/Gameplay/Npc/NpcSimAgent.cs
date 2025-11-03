using System;
using Client.Gameplay.Character;
using Client.Gameplay.Health;
using Client.Gameplay.Map;
using Client.Gameplay.Movement;
using Client.Gameplay.Npc.Network;
using Client.Services.Injections;
using UnityEngine;

namespace Client.Gameplay.Npc
{
    public class NpcSimAgent : MonoBehaviour, IChunkSimEntity, IStateProvider<NpcState>
    {
        [SerializeField] private SimpleRider _rider;
        [SerializeField] private float _arriveStopDist = 0.3f;

        public uint Id { get; private set; }
        public bool IsActive => gameObject.activeInHierarchy;
        public Vector3 Position => transform.position;

        private Transform _tr;
        private Transform _target;
        private ICharacterContainer _characterContainer;
        private GameplayContextBehaviour _gameplayContext;

        private void Awake()
        {
            _tr = transform;
            _characterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
            _gameplayContext = GameplayContextBehaviour.Instance;
        }

        private void FixedUpdate()
        {
            _rider.ApplyMovementXZ();
            _rider.ApplyRotationY();
        }

        public void Init(in NpcSpawnData data)
        {
            Id = data.Id;
            if (_characterContainer.TryGet(data.TargetId, out var characterContext))
            {
                _target = characterContext.transform;
                _rider.SetLookTarget(_target);
            }

            _rider.ChangeSpeed(data.Stats.MoveSpeed, data.Stats.MaxSpeed);
        }

        public void Deactivate()
        {
            _target = null;
        }

        public void Simulate(float delta)
        {
            var direction = Vector2.zero;
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

        public NpcState ExtractState(uint tick)
        {
            var ks = _rider.GetState();
            return new NpcState
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
            if (!other.transform.TryGetComponent<CharacterHealthController>(out var healthController))
            {
                return;
            }

            healthController.Damage(2);
            if (healthController.IsDead)
            {
                // TODO: Player is dead
            }

            _gameplayContext.NpcSpawner.DespawnNpc(Id);
        }
    }
}