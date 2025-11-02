using Client.Gameplay.Character.Network;
using Client.Gameplay.Projectile.Network;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Character
{
    public class CharacterAttack : NetworkBehaviour
    {
        [SerializeField] private CharacterContext _characterContext;
        [SerializeField] private float _cooldownDuration = 0.5f;

        private ProjectileSpawner _projectileSpawner;

        private float _cooldownLeft;
        private bool _isHost;

        private Transform _tr;
        private GameplayContextBehaviour _gameplayContext;
        private Transform Tr => _tr != null ? _tr : _tr = transform;

        private void Awake()
        {
            _cooldownLeft = _cooldownDuration;
            if (_characterContext == null) Tr.GetComponent<CharacterContext>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _gameplayContext = GameplayContextBehaviour.Instance;
            _projectileSpawner = _gameplayContext.ProjectileSpawner;

            _isHost = IsServerInitialized;
        }

        private void Update()
        {
            if (!_isHost)
            {
                return;
            }

            _cooldownLeft -= Time.deltaTime;
            if (_cooldownLeft > 0)
            {
                return;
            }

            Fire();
            _cooldownLeft = _cooldownDuration;
        }

        [Server]
        private void Fire()
        {
            var position = Tr.position;
            if (!_gameplayContext.TryFindNearestEnemy(position, out var nearestEntityId, out var nearestEntityGhost))
            {
                return;
            }

            var direction = (nearestEntityGhost.transform.position - position);
            direction.y = direction.z;
            _projectileSpawner.SpawnRandomProjectile(_characterContext.ObjectId, nearestEntityId, direction);
        }
    }
}