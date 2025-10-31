using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public class KeyboardMovementInput : IMovementInputHandler
    {
        public Vector3 Move(float delta)
        {
            return new Vector3(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
        }
    }
}