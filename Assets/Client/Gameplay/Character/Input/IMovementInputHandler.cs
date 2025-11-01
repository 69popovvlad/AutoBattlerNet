using UnityEngine;

namespace Client.Gameplay.Character.Input
{
    public interface IMovementInputHandler
    {
        Vector2 Read(float delta);
    }
}