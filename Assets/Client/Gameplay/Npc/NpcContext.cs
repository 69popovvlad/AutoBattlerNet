using Client.Gameplay.Health;
using Client.Gameplay.Npc.Network;
using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Npc
{
    public class NpcContext : MonoBehaviour, IPoolable, IKeyed<uint>
    {
        [SerializeField] private NpcSimAgent _npcSimAgent;
        [SerializeField] private NpcGhost _ghost;
        [SerializeField] private HealthController _healthController;

        private NpcStats _stats;
        private Transform Tr => _tr != null ? _tr : _tr = transform;
        private Transform _tr;

        public NpcSimAgent NpcSimAgent => _npcSimAgent;
        public NpcGhost Ghost => _ghost;
        public uint Id => _npcSimAgent.Id;
        public uint Key => Id;

        internal void Init(in NpcSpawnData data)
        {
            _npcSimAgent.Init(data);
            _healthController.SetMaxHealth(data.Stats.Health);
            _healthController.Init(data.Id);

            switch (data.Stats.TypeId)
            {
                // TODO: change color or smth here
            }
        }

        internal void TeleportToPoint(in Vector3 position) =>
            Tr.position = position;

        public void OnRent()
        {
            gameObject.SetActive(true);
        }


        public void OnReturn()
        {
            gameObject.SetActive(false);

            _npcSimAgent.Deactivate();
        }
    }
}