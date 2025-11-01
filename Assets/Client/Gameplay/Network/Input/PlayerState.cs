using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public struct PlayerState
    {
        public uint LastSequence;
        public Vector3 Position;
        public Vector3 Velocity;
        public float Yaw;
    }
}