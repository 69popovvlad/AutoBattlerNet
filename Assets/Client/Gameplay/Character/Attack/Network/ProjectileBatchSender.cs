using Client.Gameplay.Map;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Character.Attack.Network
{
    public class ProjectileBatchSender : NetworkBehaviour, IStateBatchSink<ProjectileState>
    {
        [SerializeField] private ProjectileNetClient _client;

        private bool _isHost;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _isHost = IsServerInitialized;
        }

        public void SendBatch(uint tick, ProjectileState[] items)
        {
            if (!_isHost)
            {
                return;
            }

            var batch = new ProjectileStateBatch
            {
                Tick = tick,
                Items = items
            };
            BroadcastBatchObserversRpc(batch);
        }

        [ObserversRpc(BufferLast = false)]
        private void BroadcastBatchObserversRpc(ProjectileStateBatch batch)
        {
            _client.ConsumeBatch(batch);
        }
    }
}