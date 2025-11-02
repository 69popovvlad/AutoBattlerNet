using System;
using Client.Gameplay.Map;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Client.Gameplay.Npc.Network
{
    [ExecuteAlways]
    public partial class NpcAuthority
    {
        [Header("Grid")]
        [SerializeField] private int _width = 100;

        [SerializeField] private int _height = 100;

        [SerializeField, Tooltip("Chunk's axis offset")]
        private int _minX = -50;

        [SerializeField, Tooltip("Chunk's axis offset")]
        private int _minZ = -50;

        [Header("Chunk")]
        [SerializeField, Tooltip("In game units")]
        private float _chunkSize = 1f;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;

        [SerializeField] private Color _gridColor = Color.green;

        [SerializeField, Tooltip("Show chunks coordinates")]
        private bool _drawLabels = false;

        private ChunkGrid _chunkGrid;

        protected override void OnValidate()
        {
            base.OnValidate();

            RebuildGrid();
        }

        private void Awake()
        {
            RebuildGrid();
        }

        private void RebuildGrid()
        {
            _width = Mathf.Max(1, _width);
            _height = Mathf.Max(1, _height);
            _chunkSize = Mathf.Max(0.0001f, _chunkSize);

            _chunkGrid = new ChunkGrid(_width, _height, _chunkSize, _minX, _minZ);
        }

        public void Add(SomeNpc npc)
        {
            if (npc == null)
            {
                return;
            }

            if (!_chunkGrid.TryAddEntityAtWorld(npc.Id, npc.transform.position))
            {
                throw new Exception(
                    $"Failed to add entity because position {npc.transform.position} is outside the grid");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
            {
                return;
            }

            if (_chunkGrid == null)
            {
                RebuildGrid();
            }

            Gizmos.color = _gridColor;

            // origin — left-bottom corner (in world coordinates) corresponding to (minX, minZ)
            var origin = new Vector3(_minX * _chunkSize, 0f, _minZ * _chunkSize);

            var worldWidth = _width * _chunkSize;
            var worldHeight = _height * _chunkSize;

            // Vertical lines (X axis)
            for (var x = 0; x <= _width; ++x)
            {
                var a = origin + new Vector3(x * _chunkSize, 0f, 0f);
                var b = a + new Vector3(0f, 0f, worldHeight);
                Gizmos.DrawLine(a, b);
            }

            // Horizontal lines (Z axis)
            for (var z = 0; z <= _height; ++z)
            {
                var a = origin + new Vector3(0f, 0f, z * _chunkSize);
                var b = a + new Vector3(worldWidth, 0f, 0f);
                Gizmos.DrawLine(a, b);
            }

#if UNITY_EDITOR
            if (!_drawLabels)
            {
                return;
            }

            Handles.color = _gridColor;
            // Small captions in the center of each chunk (can be slow with large grids)
            for (var z = 0; z < _height; ++z)
            for (var x = 0; x < _width; ++x)
            {
                var chunkX = _minX + x;
                var chunkZ = _minZ + z;
                var center = origin + new Vector3((x + 0.5f) * _chunkSize, 0f, (z + 0.5f) * _chunkSize);
                Handles.Label(center + Vector3.up * 0.1f, $"{chunkX},{chunkZ}");
            }
#endif
        }
    }
}