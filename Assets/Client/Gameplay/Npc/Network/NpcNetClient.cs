using System.Collections.Generic;
using UnityEngine;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Npc.Network
{
    [ContainableServiceImplementation("f9bc6acc-c611-42af-8a60-f64739272832", "9b18d07c-994c-463a-b429-15c0e04bf55d")]
    public class NpcNetClient : MonoBehaviour, INpcGhostContainer
    {
        private readonly Dictionary<uint, NpcGhost> _byId = new();

        public void Register(uint id, NpcGhost ghost) => _byId[id] = ghost;

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
                }
            }
        }

        public bool TryGetGhost(uint entityId, out NpcGhost ghost) =>
            _byId.TryGetValue(entityId, out ghost);
    }
}