using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class KeyboardMovementInput : IMovementInputHandler
    {
        public Vector2 Read(float delta)
        {
            return new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
        }
    }
}