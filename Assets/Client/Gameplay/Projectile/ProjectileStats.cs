using System;

namespace Client.Gameplay.Projectile
{
    [Serializable]
    public struct ProjectileStats
    {
        public ushort TypeId;
        public int Damage;
        public float Speed;
        public float MaxSpeed;
    }
}