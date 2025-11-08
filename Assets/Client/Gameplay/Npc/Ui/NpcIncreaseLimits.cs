using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Client.Gameplay.Npc.Ui
{
    public class NpcIncreaseLimits : MonoBehaviour
    {
        [SerializeField] private int    _npcIncreaseCount = 50;
        [SerializeField] private Button _moreNpcButton;

        private GameplayContextBehaviour _gameplayContext;

        private void Awake()
        {
            _moreNpcButton.onClick.AddListener(OnIncreaseNpcLimitsButtonClicked);
        }

        private void Start()
        {
            _gameplayContext = GameplayContextBehaviour.Instance;
        }

        private void OnDestroy()
        {
            _moreNpcButton.onClick.RemoveListener(OnIncreaseNpcLimitsButtonClicked);
        }

        private void OnIncreaseNpcLimitsButtonClicked()
        {
            _gameplayContext.NpcSpawner.SpawnMoreEnemies(_npcIncreaseCount);
        }
    }
}