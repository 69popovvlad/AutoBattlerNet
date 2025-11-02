using Client.Gameplay.Network.Character;
using Client.Gameplay.Npc.Network;
using Client.Services.Injections;
using Client.Services.Pool;
using UnityEngine;
using Validosik.Client.Character.Rider;

namespace Client.Gameplay.Npc
{
    public class SomeNpc : MonoBehaviour, IPoolable
    {
        [SerializeField] private GroundRider _groundRider;
        [SerializeField] private NpcGhost _ghost;

        private NpcStats _stats;
        private Transform Tr => _tr != null ? _tr : _tr = transform;
        private ICharacterContainer _characterContainer;
        private CharacterContext _targetContext;
        private Transform _tr;

        public uint Id { get; private set; }
        public NpcStats Stats => _stats;

        public NpcGhost Ghost => _ghost;
        public bool HasTarget => _targetContext != null;
        public Vector3 TargetPosition => _targetContext ? _targetContext.transform.position : Tr.position;
        public GroundRider Rider => _groundRider;

        internal void SetStats(in NpcStats stats)
        {
            _stats = stats;
            _characterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }

        internal void TeleportToPoint(in Vector3 position) => Tr.position = position;

        internal void Activate(uint entityId, int targetId)
        {
            Id = entityId;

            if (!_characterContainer.TryGet(targetId, out _targetContext))
            {
                _targetContext = null;
            }
        }

        public void OnRent()
        {
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            // TODO: Unregister npc here
            // NpcAuthority.UnregisterNpc(this);
            // NpcNetClient.Unregister(Id);

            gameObject.SetActive(false);
        }
    }
}