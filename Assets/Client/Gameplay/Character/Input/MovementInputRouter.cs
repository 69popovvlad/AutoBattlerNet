using Client.Gameplay.Network.Input;
using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class MovementInputRouter : MonoBehaviour
    {
        [SerializeField] private PlayerNetworkInput _playerNetworkInput;
        private IMovementInputHandler _movementInputHandler;

        public void Initialize(IMovementInputHandler handler)
        {
            _movementInputHandler = handler;
        }

        private void Update()
        {
            if (_movementInputHandler == null)
            {
                return;
            }

            var inputDirection = _movementInputHandler.Read(Time.deltaTime);
            _playerNetworkInput.CaptureLocalInput(inputDirection, Time.deltaTime, 0);
        }
    }
}