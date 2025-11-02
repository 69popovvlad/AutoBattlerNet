using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Npc
{
    public class NpcPool : TrackedPool<uint, NpcContext>
    {
        [SerializeField] private NpcContext _prefab;
        [SerializeField] private Transform _poolRoot;

        protected override NpcContext CreateInstance() =>
            Instantiate(_prefab, _poolRoot);
    }
}