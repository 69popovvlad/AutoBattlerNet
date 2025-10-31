using Client.Gameplay.Network.Character;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Player
{
    public class PlayerBootstrapper : NetworkBehaviour
    {
        [SerializeField] private CharacterContext _characterPrefab;

        private CharacterContext _character;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                RequestSpawnCharacterServerRpc();
            }
        }

        [ServerRpc]
        private void RequestSpawnCharacterServerRpc()
        {
            var character = Instantiate(_characterPrefab, gameObject.transform);
            ServerManager.Spawn(character, Owner);
        }
    }
}