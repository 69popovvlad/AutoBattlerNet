using Client.Gameplay.Character;
using Client.Gameplay.Character.Ui;
using Client.Gameplay.Npc;
using Client.Gameplay.Npc.Network;
using Client.Gameplay.Npc.Views;
using Client.Gameplay.Projectile.Network;
using Client.Services.Injections;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay
{
    [DefaultExecutionOrder(-1000)]
    public class GameplayContextBehaviour : NetworkBehaviour
    {
        public static GameplayContextBehaviour Instance { get; private set; }

        public NpcAuthority NpcAuthority;
        public NpcNetClient NpcNetClient;
        public NpcSpawner NpcSpawner;
        public NpcMaterialHolder NpcMaterialHolder;
        public ProjectileSpawner ProjectileSpawner;

        public CharacterDamageFlasher CharacterDamageFlasher;

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

        public bool TryFindNearestEnemy(in Vector3 position, out uint nearestEntityId, out NpcContext nearestEntity)
        {
            var chunkGrid = NpcAuthority.ChunkGrid;
            var chunkIndex = chunkGrid.ToIndexFromWorld(position.x, position.z);
            var entities = chunkGrid.GetEntitiesInChunkByIndex(chunkIndex);

            nearestEntityId = 0;
            nearestEntity = default;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < entities.Count; ++i)
            {
                var entityId = entities[i];
                if (!NpcSpawner.TryGetSpawned(entityId, out var entity))
                {
                    continue;
                }

                var distance = (position - entity.transform.position).sqrMagnitude;
                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                nearestEntityId = entityId;
                nearestEntity = entity;
            }

            if (nearestEntity == null)
            {
                return false;
            }

            return true;
        }
    }
}