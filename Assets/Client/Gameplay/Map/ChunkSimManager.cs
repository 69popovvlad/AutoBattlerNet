using System.Collections.Generic;
using UnityEngine;

namespace Client.Gameplay.Map
{
    /// Generic manager for chunked simulation & batched state publishing
    public abstract class ChunkSimManager<TState> : MonoBehaviour
    {
        [Header("Simulation")]
        [SerializeField, Range(1, 120)] private int _simTargetFPS = 30;

        [SerializeField, Range(1, 120)] private int _sendTargetFPS = 20;

        [Header("Refs")]
        [SerializeField] private ChunkGridBehaviour _gridBehaviour;

        private readonly List<IChunkSimEntity> _entities = new();
        private readonly List<IStateProvider<TState>> _providers = new();
        private readonly List<TState> _dirty = new();

        private float _simDelta, _sendDelta, _simAccum, _sendAccum;
        private uint _tick;

        private IStateBatchSink<TState> _sink;

        public uint Tick => _tick;
        public ChunkGridBehaviour GridBehaviour => _gridBehaviour;

        private void Awake()
        {
            _simDelta = 1f / Mathf.Max(1, _simTargetFPS);
            _sendDelta = 1f / Mathf.Max(1, _sendTargetFPS);

            if (_gridBehaviour == null)
            {
                _gridBehaviour = GetComponentInChildren<ChunkGridBehaviour>();
            }
        }

        public void SetSink(IStateBatchSink<TState> sink) => _sink = sink;

        public void Register<TAgent>(TAgent agent)
            where TAgent : Component, IChunkSimEntity, IStateProvider<TState>
        {
            if (_entities.Contains(agent))
            {
                return;
            }

            _entities.Add(agent);
            _providers.Add(agent);

            _gridBehaviour.ChunkGrid.TryAddEntityAtWorld(agent.Id, agent.Position);
        }

        public void Unregister<TAgent>(TAgent agent)
            where TAgent : Component, IChunkSimEntity, IStateProvider<TState>
        {
            var index = _entities.IndexOf(agent);
            if (index < 0)
            {
                return;
            }

            _entities.RemoveAt(index);
            _providers.RemoveAt(index);
            _gridBehaviour.ChunkGrid.RemoveEntity(agent.Id);
        }

        private void Update()
        {
            _simAccum += Time.deltaTime;
            while (_simAccum >= _simDelta)
            {
                ++_tick;
                SimulateStep(_simDelta);
                _simAccum -= _simDelta;
            }

            _sendAccum += Time.deltaTime;
            if (_sendAccum < _sendDelta)
            {
                return;
            }

            _sendAccum = 0f;
            if (_dirty.Count <= 0 || _sink == null)
            {
                return;
            }

            _sink.SendBatch(_tick, _dirty.ToArray());
            _dirty.Clear();
        }

        private void SimulateStep(float dt)
        {
            var grid = _gridBehaviour.ChunkGrid;

            for (var i = 0; i < _entities.Count; ++i)
            {
                var entity = _entities[i];
                if (entity is not { IsActive: true })
                {
                    continue;
                }

                entity.Simulate(dt);

                if (grid != null && !grid.TryMoveEntityAtWorld(entity.Id, entity.Position))
                {
                    return;
                }

                // Mark as dirty
                var provider = _providers[i];
                _dirty.Add(provider.ExtractState(_tick));
            }
        }
    }
}