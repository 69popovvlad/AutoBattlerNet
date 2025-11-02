using Client.Gameplay.Character;
using Client.Services.Injections;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Client.Gameplay.Npc.Network
{
    public class NpcSpawner : NetworkBehaviour
    {
        [SerializeField] private NpcPool _pool;
        [SerializeField] private int _npcLimit = 50;
        [SerializeField] private float _spawnInterval = 1;
        [SerializeField] private NpcStats[] _stats;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private NpcAuthority _npcAuthority;
        [SerializeField] private NpcNetClient _npcNetClient;

        private readonly SyncVar<uint> _nextId = new SyncVar<uint>(0);
        private uint _nextSpawnPoint;
        private bool _isHost;
        private readonly SyncVar<float> _leftToSpawn = new SyncVar<float>(float.MaxValue);
        private ICharacterContainer _characterContainer;

        public float TotalSpawned => _nextId.Value;
        public float SpawnedCount => _pool.SpawnedCount;
        public float SpawnProgress => Mathf.Clamp((_spawnInterval - _leftToSpawn.Value) / _spawnInterval, 0, 1);
        
        public bool TryGetSpawned(uint key, out NpcContext value) =>
            _pool.TryGetSpawned(key, out value);
        
        private void Awake()
        {
            _leftToSpawn.Value = _spawnInterval;

            _characterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }

        private void Start()
        {
            _pool.Prewarm(_npcLimit);
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _isHost = IsServerInitialized;
        }

        private void Update()
        {
            if (!_isHost)
            {
                return;
            }

            if (_pool.SpawnedCount >= _npcLimit)
            {
                return;
            }

            _leftToSpawn.Value -= Time.deltaTime;
            if (_leftToSpawn.Value > 0)
            {
                return;
            }

            SpawnRandomNpc();
            _leftToSpawn.Value = _spawnInterval;
        }

        [Server(Logging = LoggingType.Off)]
        private void SpawnRandomNpc()
        {
            if (!_characterContainer.TryGetRandom(out var targetContext))
            {
                return;
            }

            var randomStats = _stats[Random.Range(0, _stats.Length)];
            var randomSpawnPoint = Random.Range(0, _spawnPoints.Length);
            SpawnOnNetwork(new NpcSpawnData()
            {
                Id = _nextId.Value++,
                TargetId = targetContext.NetworkObject.ObjectId,
                SpawnPoint = randomSpawnPoint,
                Stats = randomStats,
            });
        }

        [ObserversRpc]
        private void SpawnOnNetwork(NpcSpawnData data)
        {
            var npc = _pool.Rent();
            npc.Init(data);
            _pool.Register(npc);

            npc.TeleportToPoint(_spawnPoints[data.SpawnPoint].position);

            if (_isHost)
            {
                _npcAuthority.RegisterNpc(npc.NpcSimAgent);
            }
            else
            {
                _npcNetClient.Register(npc.Id, npc.Ghost);
            }
        }

        [Server(Logging = LoggingType.Off)]
        public void DespawnNpc(uint npcId)
        {
            DespawnOnNetwork(new NpcDespawnData()
            {
                Id = npcId
            });
        }

        [ObserversRpc]
        private void DespawnOnNetwork(NpcDespawnData data)
        {
            if (!_pool.TryGetSpawned(data.Id, out var npc))
            {
                return;
            }

            if (_isHost)
            {
                _npcAuthority.UnregisterNpc(npc.NpcSimAgent);
            }
            else
            {
                _npcNetClient.Unregister(npc.Id);
            }

            _pool.Return(npc);
        }
    }
}