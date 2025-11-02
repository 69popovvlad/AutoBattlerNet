using Client.Gameplay.Character;
using Client.Gameplay.Character.Attack.Network;
using Client.Gameplay.Npc.Network;
using Client.Services.Injections;
using UnityEngine;

namespace Client.Gameplay
{
    [DefaultExecutionOrder(-1000)]
    public class GameplayContextBehaviour : MonoBehaviour
    {
        public static GameplayContextBehaviour Instance { get; private set; }

        public NpcAuthority NpcAuthority;
        public NpcNetClient NpcNetClient;
        public ProjectileSpawner ProjectileSpawner;
        
        public ICharacterContainer CharacterContainer;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CharacterContainer = Ioc.Instance.Resolve<ICharacterContainer>();
        }
    }
}