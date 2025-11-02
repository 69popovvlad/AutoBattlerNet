using UnityEngine;

namespace Client.Gameplay.Projectile.Network
{
    public class ProjectileSpawnData
    {
        public uint Id;
        public int OwnerId;
        public uint TargetId;
        public Vector2 Direction;
        public ProjectileStats Stats;
    }
}