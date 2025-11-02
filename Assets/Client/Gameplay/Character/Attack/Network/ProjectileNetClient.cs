using System.Collections.Generic;
using UnityEngine;

namespace Client.Gameplay.Character.Attack.Network
{
    public class ProjectileNetClient : MonoBehaviour
    {
        private readonly Dictionary<uint, ProjectileGhost> _byId = new();

        public void Register(uint id, ProjectileGhost ghost) => _byId[id] = ghost;

        public void Unregister(uint id)
        {
            _byId.Remove(id);
        }

        public void ConsumeBatch(ProjectileStateBatch batch)
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
    }
}