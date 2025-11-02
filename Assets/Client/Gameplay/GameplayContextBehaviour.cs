using Client.Gameplay.Character;
using Client.Gameplay.Npc.Network;
using Client.Gameplay.Projectile.Network;
using Client.Services.Injections;
using UnityEngine;

namespace Client.Gameplay
{
    [DefaultExecutionOrder(-1000)]
    public class GameplayContextBehaviour : MonoBehaviour
    {
        public static GameplayContextBehaviour Instance { get; private set; }

        public NpcAuthority NpcAuthority;
        public NpcNetClient NpcNetClient;
        public NpcSpawner NpcSpawner;
        public ProjectileSpawner ProjectileSpawner;

        public ICharacterContainer CharacterContainer;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CharacterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }

        public bool TryFindNearestEnemy(in Vector3 position, out uint nearestEntityId, out NpcGhost nearestEntityGhost)
        {
            var chunkGrid = NpcAuthority.ChunkGrid;
            var chunkIndex = chunkGrid.ToIndexFromWorld(position.x, position.z);
            var chunkPos = chunkGrid.FromIndex(chunkIndex);
            var entities = chunkGrid.GetEntitiesInChunk(chunkPos.x, chunkPos.z);

            var nearestIndex = -1;
            nearestEntityId = 0;
            nearestEntityGhost = default;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < entities.Count; ++i)
            {
                var entityId = entities[i];
                if (!NpcNetClient.TryGetGhost(entityId, out var ghost))
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
                return false;
            }

            return true;
        }
    }
}