using System;

namespace Client.Gameplay.Character.Attack
{
    [Serializable]
    public struct ProjectileStats
    {
        public ushort TypeId;
        public int Damage;
    }
}