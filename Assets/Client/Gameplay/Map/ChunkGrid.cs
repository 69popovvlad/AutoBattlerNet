using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Gameplay.Map
{
    public class ChunkGrid
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _chunkSize;
        private readonly int _minX;
        private readonly int _minZ;

        /// Chunks array with entities list in it
        private readonly List<uint>[] _chunks;

        /// Map entityId -> (chunkIndex, indexInList)
        private readonly Dictionary<uint, (int chunkIndex, int indexInList)> _entityIndex = new();

        private readonly float _originWorldX;
        private readonly float _originWorldZ;

        public ChunkGrid(int width, int height, float chunkSize, int minX = 0, int minZ = 0)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("width/height should be greater than 0");
            }

            _width = width;
            _height = height;
            _chunkSize = chunkSize;
            _minX = minX;
            _minZ = minZ;

            _originWorldX = _minX * _chunkSize;
            _originWorldZ = _minZ * _chunkSize;

            _chunks = new List<uint>[width * height];
            for (var i = 0; i < _chunks.Length; ++i)
            {
                _chunks[i] = new List<uint>(4);
            }
        }

        /// Chunk position to index
        public int ToIndex(int x, int z)
        {
            var sx = x - _minX;
            var sz = z - _minZ;
            if ((uint)sx >= (uint)_width || (uint)sz >= (uint)_height)
            {
                throw new IndexOutOfRangeException($"Chunk coords out of range: {x},{z}");
            }

            return sx + sz * _width;
        }

        public (int x, int z) FromIndex(int index)
        {
            var sx = index % _width;
            var sz = index / _width;
            return (sx + _minX, sz + _minZ);
        }

        public void AddEntity(uint entityId, int x, int z)
        {
            var index = ToIndex(x, z);
            var list = _chunks[index];
            list.Add(entityId);
            _entityIndex[entityId] = (index, list.Count - 1);
        }

        public bool TryAddEntityAtWorld(uint entityId, Vector3 worldPos)
        {
            try
            {
                var idx = ToIndexFromWorld(worldPos.x, worldPos.z);
                var list = _chunks[idx];
                list.Add(entityId);
                _entityIndex[entityId] = (idx, list.Count - 1);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public bool RemoveEntity(uint entityId)
        {
            if (!_entityIndex.TryGetValue(entityId, out var meta))
            {
                return false;
            }

            var list = _chunks[meta.chunkIndex];
            var last = list.Count - 1;
            if (meta.indexInList != last)
            {
                var swapId = list[last];
                list[meta.indexInList] = swapId;
                _entityIndex[swapId] = (meta.chunkIndex, meta.indexInList);
            }

            list.RemoveAt(last);
            _entityIndex.Remove(entityId);
            return true;
        }

        /// Move entity to a new chunk (previously remove it from the old one)
        public bool MoveEntity(uint entityId, int newX, int newZ)
        {
            if (!_entityIndex.TryGetValue(entityId, out var meta))
            {
                return false;
            }

            var newIndex = ToIndex(newX, newZ);
            if (newIndex == meta.chunkIndex) // already there
            {
                return true;
            }

            // Remove from old chunk (without removing from the dictionary) 
            var oldList = _chunks[meta.chunkIndex];
            var last = oldList.Count - 1;
            if (meta.indexInList != last)
            {
                var swapId = oldList[last];
                oldList[meta.indexInList] = swapId;
                _entityIndex[swapId] = (meta.chunkIndex, meta.indexInList);
            }

            oldList.RemoveAt(last);

            // Place to a new one
            var newList = _chunks[newIndex];
            newList.Add(entityId);
            _entityIndex[entityId] = (newIndex, newList.Count - 1);
            return true;
        }

        public bool TryMoveEntityAtWorld(uint entityId, Vector3 worldPos)
        {
            var newChunkX = WorldToChunkCoordX(worldPos.x);
            var newChunkZ = WorldToChunkCoordZ(worldPos.z);
            return MoveEntity(entityId, newChunkX, newChunkZ);
        }

        public IReadOnlyList<uint> GetEntitiesInChunk(int x, int z)
        {
            var index = ToIndex(x, z);
            return _chunks[index];
        }

        public bool TryGetChunkIndexOfEntity(uint entityId, out int chunkIndex)
        {
            if (_entityIndex.TryGetValue(entityId, out var meta))
            {
                chunkIndex = meta.chunkIndex;
                return true;
            }

            chunkIndex = -1;
            return false;
        }

        public int ToIndexFromWorld(float worldX, float worldZ)
        {
            var cx = WorldToChunkCoordX(worldX);
            var cz = WorldToChunkCoordZ(worldZ);
            return ToIndex(cx, cz);
        }

        private int WorldToChunkCoordX(float worldX) =>
            Mathf.FloorToInt((worldX - _originWorldX) / _chunkSize) + _minX;

        private int WorldToChunkCoordZ(float worldZ) =>
            Mathf.FloorToInt((worldZ - _originWorldZ) / _chunkSize) + _minZ;
    }
}