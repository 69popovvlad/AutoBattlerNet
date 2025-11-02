using UnityEngine;

namespace Client.Gameplay.Character.Attack.Network
{
    public struct ProjectileState
    {
        public uint Tick;
        public uint Id;
        public Vector3 Position;
        public Vector3 Velocity;
        public float Yaw;
    }
}