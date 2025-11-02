using System;

namespace Client.Gameplay.Health
{
    public interface IDamageable
    {
        event Action<int> OnDamaged;
        
        void Damage(int damage);
    }
}