using Client.Gameplay.Npc.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Gameplay.Npc.Ui
{
    public class NpcCountView : MonoBehaviour
    {
        [SerializeField] private Text _spawnedCountText;
        [SerializeField] private Text _totalCountText;
        private NpcSpawner _npcSpawner;

        private void Awake()
        {
            _npcSpawner = GameplayContextBehaviour.Instance.NpcSpawner;
        }

        private void Update()
        {
            _spawnedCountText.text = $"Enemies: {_npcSpawner.SpawnedCount}";
            _totalCountText.text = $"Total spawned: {_npcSpawner.TotalSpawned}";
        }
    }
}