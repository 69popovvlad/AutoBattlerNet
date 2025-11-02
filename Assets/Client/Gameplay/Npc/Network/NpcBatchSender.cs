using Client.Gameplay.Map;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Npc.Network
{
    public class NpcBatchSender : NetworkBehaviour, IStateBatchSink<NpcState>
    {
        [SerializeField] private NpcNetClient _client;

        private bool _isHost;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _isHost = IsServerInitialized;
        }

        public void SendBatch(uint tick, NpcState[] items)
        {
            if (!_isHost)
            {
                return;
            }

            var batch = new NpcStateBatch
            {
                Tick = tick,
                Items = items
            };
            BroadcastBatchObserversRpc(batch);
        }

        [ObserversRpc(BufferLast = false)]
        private void BroadcastBatchObserversRpc(NpcStateBatch batch)
        {
            _client.ConsumeBatch(batch);
        }
    }
}