using Client.Gameplay.Camera;
using Client.Gameplay.Character.Input;
using Client.Services.Injections;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Character
{
    public class CharacterContext : NetworkBehaviour
    {
        [SerializeField]
        private MovementInputRouter _movementInputRouter;

        public override void OnStartClient()
        {
            base.OnStartClient();

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
    }
}