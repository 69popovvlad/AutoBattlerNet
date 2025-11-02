using UnityEngine;

namespace Client.Gameplay.Character.Attack.Network
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