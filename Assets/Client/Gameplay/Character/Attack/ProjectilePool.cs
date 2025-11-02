using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Character.Attack
{
    public class ProjectilePool : Pool<ProjectileContext>
    {
        [SerializeField] private ProjectileContext _prefab;
        [SerializeField] private Transform _poolRoot;

        protected override ProjectileContext CreateInstance() =>
            Instantiate(_prefab, _poolRoot);
    }
}