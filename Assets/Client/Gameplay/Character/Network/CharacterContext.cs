using Client.Gameplay.Camera;
using Client.Gameplay.Character.Input;
using Client.Gameplay.Health;
using Client.Services.Injections;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Character.Network
{
    public class CharacterContext : NetworkBehaviour
    {
        [SerializeField]
        private MovementInputRouter _movementInputRouter;

        [SerializeField] private HealthController _healthController;

        private GameplayContextBehaviour _gameplayContext;

        private void Awake()
        {
            _gameplayContext = GameplayContextBehaviour.Instance;
            _healthController.OnDamaged += OnDamaged;
            _healthController.Init(0);
        }

        private void OnDestroy()
        {
            _healthController.OnDamaged -= OnDamaged;
        }

        private void OnDamaged(int obj)
        {
            if (!IsOwner)
            {
                return;
            }

            _gameplayContext.CharacterDamageFlasher.Flash();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Ioc.Instance.Resolve<ICharacterContainer>().TryAdd(this);

            if (IsOwner)
            {
                _movementInputRouter.Initialize(new KeyboardMovementInput());
                Ioc.Instance
                    .Resolve<ICameraTargetProvider>()
                    .CameraTarget = transform;
            }
            else
            {
                _movementInputRouter.Initialize(new NoOpMovementInput());
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            Ioc.Instance.Resolve<ICharacterContainer>().Remove(this);
        }
    }
}