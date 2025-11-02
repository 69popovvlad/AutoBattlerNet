using System;
using UnityEngine;

namespace Client.Gameplay.Health
{
    public class HealthController : MonoBehaviour, IDamageable, IKillable<uint>
    {
        [SerializeField] private int _initialHealth = 10;

        private int _currentHealth;

        public event Action<int> OnDamaged;
        public event Action<uint> OnDead;

        public uint Id { get; private set; }
        public bool IsDead { get; private set; }

        public void Init(uint EntityId)
        {
            Id = EntityId;
            Reset();
        }

        public virtual void SetMaxHealth(int maxHealth)
        {
            _initialHealth = maxHealth;
            Reset();
        }

        public void Reset()
        {
            _currentHealth = _initialHealth;
            IsDead = false;
        }

        public void Damage(int damage)
        {
            if (IsDead)
            {
                return;
            }

            _currentHealth -= damage;
            OnDamaged?.Invoke(damage);

            if (_currentHealth > 0)
            {
                return;
            }

            IsDead = true;
            OnDead?.Invoke(Id);
        }
    }
}