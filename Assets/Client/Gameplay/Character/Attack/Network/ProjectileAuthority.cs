using Client.Gameplay.Map;
using UnityEngine;

namespace Client.Gameplay.Character.Attack.Network
{
    public class ProjectileAuthority : MonoBehaviour
    {
        [SerializeField] private ChunkGridBehaviour gridBehaviour;
        [SerializeField] private ChunkSimManager<ProjectileState> _manager;
        [SerializeField] private ProjectileBatchSender _sender;

        public ChunkGridBehaviour GridBehaviour => gridBehaviour;
        public ChunkSimManager<ProjectileState> Manager => _manager;

        private void Awake()
        {
            if (_manager == null) _manager = GetComponentInChildren<ChunkSimManager<ProjectileState>>();
            if (gridBehaviour == null) gridBehaviour = GetComponentInChildren<ChunkGridBehaviour>();
            if (_sender == null) _sender = GetComponentInChildren<ProjectileBatchSender>();

            _manager.SetSink(_sender);
        }

        public void RegisterProjectile(ProjectileSimAgent agent) =>
            _manager.Register(agent);

        public void UnregisterProjectile(ProjectileSimAgent agent) =>
            _manager.Unregister(agent);
    }
}