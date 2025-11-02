using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Projectile
{
    public class ProjectilePool : TrackedPool<uint, ProjectileContext>
    {
        [SerializeField] private ProjectileContext _prefab;
        [SerializeField] private Transform _poolRoot;

        protected override ProjectileContext CreateInstance() =>
            Instantiate(_prefab, _poolRoot);
    }
}