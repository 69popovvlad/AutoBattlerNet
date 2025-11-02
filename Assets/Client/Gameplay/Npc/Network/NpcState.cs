using UnityEngine;

namespace Client.Gameplay.Npc.Network
{
    public struct NpcState
    {
        public uint Tick;
        public uint Id;
        public Vector3 Position;
        public Vector3 Velocity;
        public float Yaw;
    }
}