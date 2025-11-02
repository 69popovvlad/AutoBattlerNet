using System;

namespace Client.Gameplay.Npc
{
    [Serializable]
    public struct NpcStats
    {
        public ushort TypeId;
        public float MoveSpeed;
        public float MaxSpeed;
        public int Health;
    }
}