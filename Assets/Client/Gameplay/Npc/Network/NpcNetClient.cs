using System.Collections.Generic;
using UnityEngine;

namespace Client.Gameplay.Npc.Network
{
    public class NpcNetClient : MonoBehaviour
    {
        private readonly Dictionary<uint, NpcGhost> _byId = new();
        private          NpcAuthority               _npcAuthority;

        private void Start()
        {
            _npcAuthority = GameplayContextBehaviour.Instance.NpcAuthority;
        }

        public void Register(uint id, NpcGhost ghost)
        {
            _byId[id] = ghost;
            _npcAuthority.ChunkGrid.TryAddEntityAtWorld(id, ghost.transform.position);
        }

        public void Unregister(uint id)
        {
            _byId.Remove(id);
        }

        public void ConsumeBatch(NpcStateBatch batch)
        {
            var items = batch.Items;
            if (items == null || items.Length == 0)
            {
                return;
            }

            for (var i = 0; i < items.Length; ++i)
            {
                ref readonly var state = ref items[i];
                if (_byId.TryGetValue(state.Id, out var ghost) && ghost)
                {
                    ghost.ApplyServerState(state);
                    _npcAuthority.ChunkGrid.TryMoveEntityAtWorld(state.Id, ghost.transform.position);
                }
            }
        }
    }
}