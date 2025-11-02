using UnityEngine;

namespace Client.Gameplay.Npc.Views
{
    public class NpcMaterialHolder : MonoBehaviour
    {
        [SerializeField] private Material[] _materials;

        public Material GetMaterial(ushort typeId) => _materials[typeId];
    }
}