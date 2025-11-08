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

        public NpcAuthority      NpcAuthority;
        public NpcNetClient      NpcNetClient;
        public NpcSpawner        NpcSpawner;
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
            var currentChunkIndex = chunkGrid.ToIndexFromWorld(position.x, position.z);

            nearestEntityId = 0;
            nearestEntity = default;
            var bestDistance = float.MaxValue;

            // Current chunk check
            if (TryFindNearestInChunk(currentChunkIndex, position, ref nearestEntityId, ref nearestEntity,
                    ref bestDistance))
            {
                return true;
            }

            var (currentChunkX, currentChunkZ) = chunkGrid.FromIndex(currentChunkIndex);

            // Collect all adjacent chunks (3x3 = 9 chunks around the current one)
            var neighborChunks = new System.Collections.Generic.List<(int index, float distanceSqr)>();

            for (var offsetX = -1; offsetX <= 1; ++offsetX)
            {
                for (var offsetZ = -1; offsetZ <= 1; ++offsetZ)
                {
                    if (offsetX == 0 && offsetZ == 0)
                    {
                        continue; // Skip current chunk
                    }

                    var neighborX = currentChunkX + offsetX;
                    var neighborZ = currentChunkZ + offsetZ;

                    try
                    {
                        var neighborIndex = chunkGrid.ToIndex(neighborX, neighborZ);

                        var chunkDistanceSqr = GetDistanceToChunkSqr(position, neighborX, neighborZ);
                        neighborChunks.Add((neighborIndex, chunkDistanceSqr));
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        // Chunk is out of grid
                        continue;
                    }
                }
            }

            neighborChunks.Sort((a, b) => a.distanceSqr.CompareTo(b.distanceSqr));
            foreach (var (chunkIndex, _) in neighborChunks)
            {
                if (TryFindNearestInChunk(chunkIndex, position, ref nearestEntityId, ref nearestEntity,
                        ref bestDistance))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryFindNearestInChunk(int chunkIndex, Vector3 position, ref uint nearestEntityId,
            ref NpcContext nearestEntity, ref float bestDistance)
        {
            var entities = NpcAuthority.ChunkGrid.GetEntitiesInChunkByIndex(chunkIndex);
            var foundAny = false;

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
                foundAny = true;
            }

            return foundAny;
        }

        private float GetDistanceToChunkSqr(Vector3 position, int chunkX, int chunkZ)
        {
            var chunkGrid = NpcAuthority.ChunkGrid;
            var chunkSize = chunkGrid.ChunkSize;

            // Calculate chunk bounds in world space
            var chunkMinX = chunkX * chunkSize;
            var chunkMaxX = chunkMinX + chunkSize;
            var chunkMinZ = chunkZ * chunkSize;
            var chunkMaxZ = chunkMinZ + chunkSize;

            // Find closest point on chunk to player position
            var closestX = Mathf.Clamp(position.x, chunkMinX, chunkMaxX);
            var closestZ = Mathf.Clamp(position.z, chunkMinZ, chunkMaxZ);

            // Return squared distance to closest point
            var dx = position.x - closestX;
            var dz = position.z - closestZ;

            return dx * dx + dz * dz;
        }
    }
}