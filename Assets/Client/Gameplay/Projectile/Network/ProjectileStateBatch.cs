namespace Client.Gameplay.Projectile.Network
{
    public struct ProjectileStateBatch
    {
        public uint Tick;
        public ProjectileState[] Items;
    }
}