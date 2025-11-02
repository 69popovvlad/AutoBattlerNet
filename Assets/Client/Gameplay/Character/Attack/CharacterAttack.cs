using Client.Gameplay.Character.Attack.Network;
using Client.Gameplay.Character.Network;
using Client.Gameplay.Npc.Network;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Character.Attack
{
    public class CharacterAttack : NetworkBehaviour
    {
        [SerializeField] private CharacterContext _characterContext;
        [SerializeField] private float _cooldownDuration = 0.5f;

        private NpcAuthority _npcAuthority;
        private NpcNetClient _npcNetClient;
        private ProjectileSpawner _projectileSpawner;

        private float _cooldownLeft;
        private bool _isHost;

        private Transform _tr;
        private Transform Tr => _tr != null ? _tr : _tr = transform;

        private void Awake()
        {
            _cooldownLeft = _cooldownDuration;
            if (_characterContext == null) Tr.GetComponent<CharacterContext>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            var gameplayContext = GameplayContextBehaviour.Instance;
            _npcAuthority = gameplayContext.NpcAuthority;
            _npcNetClient = gameplayContext.NpcNetClient;
            _projectileSpawner = gameplayContext.ProjectileSpawner;

            _isHost = IsServerInitialized;
        }

        private void Update()
        {
            if (!_isHost)
            {
                return;
            }

            _cooldownLeft -= Time.deltaTime;
            if (_cooldownLeft > 0)
            {
                return;
            }

            Fire();
            _cooldownLeft = _cooldownDuration;
        }

        [Server]
        private void Fire()
        {
            var chunkGrid = _npcAuthority.ChunkGrid;
            var position = Tr.position;
            var chunkIndex = chunkGrid.ToIndexFromWorld(position.x, position.z);
            var chunkPos = chunkGrid.FromIndex(chunkIndex);
            var entities = chunkGrid.GetEntitiesInChunk(chunkPos.x, chunkPos.z);

            var nearestIndex = -1;
            uint nearestEntityId = 0;
            NpcGhost nearestEntityGhost = default;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < entities.Count; ++i)
            {
                var entityId = entities[i];
                if (!_npcNetClient.TryGetGhost(entityId, out var ghost))
                {
                    continue;
                }

                var distance = (ghost.transform.position - position).sqrMagnitude;
                if (distance >= bestDistance)
                {
                    continue;
                }

                nearestIndex = i;
                bestDistance = distance;
                nearestEntityId = entityId;
                nearestEntityGhost = ghost;
            }

            if (nearestIndex == -1 || nearestEntityGhost == null)
            {
                return;
            }

            var direction = (nearestEntityGhost.transform.position - position);
            direction.y = direction.z;
            _projectileSpawner.SpawnRandomProjectile(_characterContext.ObjectId, nearestEntityId, direction);
        }
    }
}