using Client.Services.Pool;
using UnityEngine;
using Validosik.Client.Character.Rider;

namespace Client.Gameplay.Npc
{
    public class SomeNpc: MonoBehaviour, IPoolable
    {
        [SerializeField] private GroundRider _groundRider;
        private NpcStats _stats;
        private Transform _tr;

        private void Awake()
        {
            _tr = transform;
        }

        internal void SetStats(in NpcStats stats)
        {
            _stats = stats;
        }

        internal void TeleportToPoint(in Vector3 position)
        {
            _tr.position = position;
        }

        public void OnRent()
        {
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            gameObject.SetActive(false);
        }
    }
}