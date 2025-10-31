using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public interface IMovementInputHandler
    {
        Vector3 Move(float delta);
    }
}