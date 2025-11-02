using Client.Gameplay.Character.Attack.Network;
using Client.Gameplay.Character.Network;
using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Character.Attack
{
    public class ProjectileContext: MonoBehaviour, IPoolable
    {
        [SerializeField] private ProjectileSimAgent _projectileSimAgent;
        [SerializeField] private ProjectileGhost _ghost;

        private Transform Tr => _tr != null ? _tr : _tr = transform;
        private Transform _tr;

        public ProjectileSimAgent ProjectileSimAgent => _projectileSimAgent;
        public ProjectileGhost Ghost => _ghost;
        public uint Id => _projectileSimAgent.Id;


        internal void Init(in ProjectileSpawnData data, CharacterContext characterContext)
        {
            _projectileSimAgent.Init(data);

            TeleportToPoint(characterContext.transform.position);
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

            _projectileSimAgent.Deactivate();

            // TODO: Unregister projectile here
            // ProjectileAuthority.UnregisterProjectile(this);
            // ProjectileNetClient.Unregister(Id);
        }
    }
}