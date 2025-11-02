using System;
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
        [SerializeField] private MeshRenderer _meshRenderer;

        private NpcStats _stats;
        private Transform Tr => _tr != null ? _tr : _tr = transform;
        private Transform _tr;
        private GameplayContextBehaviour _gameplayContext;

        public NpcSimAgent NpcSimAgent => _npcSimAgent;
        public NpcGhost Ghost => _ghost;
        public uint Id => _npcSimAgent.Id;
        public uint Key => Id;

        private void Awake()
        {
            _gameplayContext = GameplayContextBehaviour.Instance;
        }

        internal void Init(in NpcSpawnData data)
        {
            _npcSimAgent.Init(data);
            _healthController.SetMaxHealth(data.Stats.Health);
            _healthController.Init(data.Id);

            _meshRenderer.material = _gameplayContext.NpcMaterialHolder.GetMaterial(data.Stats.TypeId);
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