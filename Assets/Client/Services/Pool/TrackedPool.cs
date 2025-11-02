using System.Collections.Generic;

namespace Client.Services.Pool
{
    public abstract class TrackedPool<TKey, TValue> : Pool<TValue>
        where TValue : class, IKeyed<TKey>
    {
        private readonly Dictionary<TKey, TValue> _spawned = new Dictionary<TKey, TValue>();

        
        ///  Don't forget to use Register method
        public override TValue Rent() => base.Rent();
        
        public void Register(TValue value)
        {
            _spawned[value.Key] = value;
        }

        public override void Return(TValue value)
        {
            base.Return(value);
            _spawned.Remove(value.Key);
        }

        public bool TryGetSpawned(TKey key, out TValue projectile) =>
            _spawned.TryGetValue(key, out projectile);
    }
}