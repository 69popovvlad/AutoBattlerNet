using System;

namespace Client.Gameplay.Projectile
{
    [Serializable]
    public struct ProjectileStats
    {
        public ushort TypeId;
        public int Damage;
    }
}