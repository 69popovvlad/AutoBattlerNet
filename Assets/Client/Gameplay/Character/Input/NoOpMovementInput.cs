using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class NoOpMovementInput : IMovementInputHandler
    {
        public Vector3 Move(float delta)
        {
            return Vector3.zero;
        }
    }
}