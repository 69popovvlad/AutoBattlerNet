using Validosik.Client.Character.Rider;
using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class MovementInputRouter : MonoBehaviour
    {
        [SerializeField] private GroundRider _characterMovement;

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

            _characterMovement.Move(_movementInputHandler.Move(Time.deltaTime));
        }
    }
}