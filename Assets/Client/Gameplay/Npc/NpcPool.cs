using Client.Services.Pool;
using UnityEngine;

namespace Client.Gameplay.Npc
{
    public class NpcPool : Pool<SomeNpc>
    {
        [SerializeField] private SomeNpc _prefab;
        [SerializeField] private Transform _poolRoot;

        protected override SomeNpc CreateInstance() =>
            Instantiate(_prefab, _poolRoot);
    }
}