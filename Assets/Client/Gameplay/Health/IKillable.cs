using System;

namespace Client.Gameplay.Health
{
    public interface IKillable<out T>
    {
        event Action<T> OnDead;

        public bool IsDead { get; }
    }
}