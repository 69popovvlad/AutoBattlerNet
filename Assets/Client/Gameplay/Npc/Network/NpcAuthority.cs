using Client.Gameplay.Map;
using UnityEngine;

namespace Client.Gameplay.Npc.Network
{
    [ExecuteAlways]
    public class NpcAuthority : MonoBehaviour
    {
        [SerializeField] private ChunkGridBehaviour gridBehaviour;
        [SerializeField] private ChunkSimManager<NpcState> _manager;
        [SerializeField] private NpcBatchSender _sender;

        public ChunkGridBehaviour GridBehaviour => gridBehaviour;
        public ChunkSimManager<NpcState> Manager => _manager;

        public ChunkGrid ChunkGrid => gridBehaviour.ChunkGrid;

        private void Awake()
        {
            if (_manager == null) _manager = GetComponentInChildren<ChunkSimManager<NpcState>>();
            if (gridBehaviour == null) gridBehaviour = GetComponentInChildren<ChunkGridBehaviour>();
            if (_sender == null) _sender = GetComponentInChildren<NpcBatchSender>();

            _manager.SetSink(_sender);
        }

        public void RegisterNpc(NpcSimAgent agent) =>
            _manager.Register(agent);

        public void UnregisterNpc(NpcSimAgent agent) =>
            _manager.Unregister(agent);
    }
}