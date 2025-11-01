using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class NoOpMovementInput : IMovementInputHandler
    {
        public Vector2 Read(float delta)
        {
            return Vector2.zero;
        }
    }
}