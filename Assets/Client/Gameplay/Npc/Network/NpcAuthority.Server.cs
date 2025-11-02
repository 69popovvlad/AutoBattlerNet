using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Npc.Network
{
    public partial class NpcAuthority : NetworkBehaviour
    {
        [Header("SERVER")]
        [SerializeField, Range(1, 240)] private int _simTargetFPS = 30;

        [SerializeField, Range(1, 120)] private int _sendingTargetFPS = 20;
        [SerializeField] private float _arriveStopDist = 0.3f;

        [SerializeField] private NpcNetClient _npcNetClient;

        private readonly List<SomeNpc> _npcs = new();
        private readonly List<NpcState> _dirty = new();
        private float _simAccum, _sendAccum, _simDt, _sendDelta;
        private bool _isHost;
        private uint _tick;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _isHost = IsServerInitialized;
            _simDt = 1f / Mathf.Max(1, _simTargetFPS);
            _sendDelta = 1f / Mathf.Max(1, _sendingTargetFPS);
        }

        private void Update()
        {
            if (!_isHost)
            {
                return;
            }

            _simAccum += Time.deltaTime;
            while (_simAccum >= _simDt)
            {
                ++_tick;
                SimulateStep(_simDt);
                _simAccum -= _simDt;
            }

            _sendAccum += Time.deltaTime;
            if (_sendAccum < _sendDelta)
            {
                return;
            }

            _sendAccum = 0f;
            if (_dirty.Count <= 0)
            {
                return;
            }

            var batch = new NpcStateBatch
            {
                Tick = _tick,
                Items = _dirty.ToArray()
            };
            _dirty.Clear();
            BroadcastBatchObserversRpc(batch);
        }

        internal void RegisterNpc(SomeNpc npc)
        {
            if (!_npcs.Contains(npc)
                && _chunkGrid.TryAddEntityAtWorld(npc.Id, npc.transform.position))
            {
                _npcs.Add(npc);
            }
        }

        internal void UnregisterNpc(SomeNpc npc) => _npcs.Remove(npc);

        private void SimulateStep(float dt)
        {
            for (var i = 0; i < _npcs.Count; ++i)
            {
                var npc = _npcs[i];
                if (!npc || !npc.HasTarget)
                {
                    continue;
                }

                var toTarget = npc.TargetPosition - npc.transform.position;
                var flat = new Vector2(toTarget.x, toTarget.z);
                var dist = flat.magnitude;

                var dir = dist > 1e-4f ? (flat / dist) : Vector2.zero;

                // Stop logic
                if (dist <= _arriveStopDist)
                {
                    dir = Vector2.zero;
                }

                npc.Rider.ApplyInputStep(dir, dt);

                // Mark dirty to send
                var kinematicState = npc.Rider.GetState();
                _dirty.Add(new NpcState
                {
                    Tick = _tick,
                    Id = npc.Id,
                    Position = kinematicState.Position,
                    Velocity = kinematicState.Velocity,
                    Yaw = kinematicState.Yaw
                });

                if (!_chunkGrid.TryMoveEntityAtWorld(npc.Id, npc.transform.position))
                {
                    throw new Exception(
                        $"Failed to move entity because position {npc.transform.position} is outside the grid");
                }
            }
        }

        [ObserversRpc(BufferLast = false)]
        private void BroadcastBatchObserversRpc(NpcStateBatch batch)
        {
            _npcNetClient.ConsumeBatch(batch);
        }
    }
}