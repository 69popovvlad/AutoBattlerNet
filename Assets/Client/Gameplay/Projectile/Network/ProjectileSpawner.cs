using Client.Gameplay.Character;
using Client.Services.Injections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Client.Gameplay.Projectile.Network
{
    public class ProjectileSpawner : NetworkBehaviour
    {
        [SerializeField] private ProjectilePool _pool;
        [SerializeField] private float _spawnInterval = 1;
        [SerializeField] private ProjectileStats[] _stats;
        [SerializeField] private ProjectileAuthority _npcAuthority;
        [SerializeField] private ProjectileNetClient _npcNetClient;

        private readonly SyncVar<float> _leftToSpawn = new SyncVar<float>(float.MaxValue);
        private uint _nextId;
        private uint _nextSpawnPoint;

        private ICharacterContainer _characterContainer;

        public float SpawnProgress => Mathf.Clamp((_spawnInterval - _leftToSpawn.Value) / _spawnInterval, 0, 1);

        private void Awake()
        {
            _leftToSpawn.Value = _spawnInterval;
            _characterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }

        private void Start()
        {
            _pool.Prewarm(32);
        }

        [Server]
        public void SpawnRandomProjectile(int fromId, uint targetId, in Vector2 direction)
        {
            var randomStats = _stats[Random.Range(0, _stats.Length)];
            SpawnOnNetwork(new ProjectileSpawnData()
            {
                Id = _nextId++,
                OwnerId = fromId,
                TargetId = targetId,
                Direction = direction,
                Stats = randomStats,
            });
        }

        [ObserversRpc]
        private void SpawnOnNetwork(ProjectileSpawnData data)
        {
            if (!_characterContainer.TryGet(data.OwnerId, out var characterContext))
            {
                Debug.LogWarning($"Couldn't find {data.OwnerId} in {nameof(ICharacterContainer)}");
                return;
            }

            var projectile = _pool.Rent();
            projectile.Init(data, characterContext);
            _pool.Register(projectile);

            _npcAuthority.RegisterProjectile(projectile.ProjectileSimAgent);
            _npcNetClient.Register(projectile.Id, projectile.Ghost);
        }

        [Server]
        public void DespawnProjectile(uint projectileId)
        {
            DespawnOnNetwork(new ProjectileDespawnData()
            {
                Id = projectileId
            });
        }

        [ObserversRpc]
        private void DespawnOnNetwork(ProjectileDespawnData data)
        {
            if (!_pool.TryGetSpawned(data.Id, out var projectile))
            {
                return;
            }

            _npcAuthority.UnregisterProjectile(projectile.ProjectileSimAgent);
            _npcNetClient.Unregister(projectile.Id);

            _pool.Return(projectile);
        }
    }
}