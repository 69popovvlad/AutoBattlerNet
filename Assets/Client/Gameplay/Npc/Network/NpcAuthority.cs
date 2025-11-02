using Client.Gameplay.Map;
using UnityEngine;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Npc.Network
{
    [ExecuteAlways]
    [ContainableServiceImplementation("5544f085-9cb5-412e-872e-4c8266131d4f", "e984b29c-5072-47dd-91bb-249988bbdb33")]
    public class NpcAuthority : MonoBehaviour, INpcChunkHandler
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