using Client.Gameplay.Network.Character;
using Client.Gameplay.Npc.Network;
using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Npc
{
    public class NpcContext : MonoBehaviour, IPoolable
    {
        [SerializeField] private NpcSimAgent _npcSimAgent;
        [SerializeField] private NpcGhost _ghost;

        private NpcStats _stats;
        private Transform Tr => _tr != null ? _tr : _tr = transform;

        private CharacterContext _targetContext;
        private Transform _tr;

        public NpcSimAgent NpcSimAgent => _npcSimAgent;
        public NpcGhost Ghost => _ghost;
        public uint Id => _npcSimAgent.Id;
        public NpcStats Stats => _stats;


        internal void SetStats(in NpcStats stats)
        {
            _stats = stats;
        }

        internal void TeleportToPoint(in Vector3 position) =>
            Tr.position = position;

        internal void Activate(uint entityId, int targetId) =>
            _npcSimAgent.Activate(entityId, targetId);

        public void OnRent()
        {
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            gameObject.SetActive(false);

            _npcSimAgent.Deactivate();

            // TODO: Unregister npc here
            // NpcAuthority.UnregisterNpc(this);
            // NpcNetClient.Unregister(Id);
        }
    }
}