using Client.Gameplay.Character;
using Client.Services.Injections;
using FishNet.Managing.Logging;
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
        private bool _isHost;

        public float SpawnProgress => Mathf.Clamp((_spawnInterval - _leftToSpawn.Value) / _spawnInterval, 0, 1);

        public bool TryGetSpawned(uint key, out ProjectileContext value) =>
            _pool.TryGetSpawned(key, out value);

        private void Awake()
        {
            _leftToSpawn.Value = _spawnInterval;
            _characterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }

        private void Start()
        {
            _pool.Prewarm(32);
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _isHost = IsServerInitialized;
        }

        [Server(Logging = LoggingType.Off)]
        public void SpawnRandomProjectile(int fromId, uint targetId)
        {
            var randomStats = _stats[Random.Range(0, _stats.Length)];
            SpawnOnNetwork(new ProjectileSpawnData()
            {
                Id = _nextId++,
                OwnerId = fromId,
                TargetId = targetId,
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


            if (_isHost)
            {
                _npcAuthority.RegisterProjectile(projectile.ProjectileSimAgent);
            }
            else
            {
                _npcNetClient.Register(projectile.Id, projectile.Ghost);
            }
        }

        [Server(Logging = LoggingType.Off)]
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

            if (_isHost)
            {
                _npcAuthority.UnregisterProjectile(projectile.ProjectileSimAgent);
            }
            else
            {
                _npcNetClient.Unregister(projectile.Id);
            }

            _pool.Return(projectile);
        }
    }
}