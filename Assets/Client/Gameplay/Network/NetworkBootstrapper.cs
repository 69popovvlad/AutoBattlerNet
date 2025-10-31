// #define NET_HOST_TEST
// #define NET_CLIENT_TEST

using FishNet.Managing;
using UnityEngine;

namespace Client.Gameplay.Network
{
    public class NetworkBootstrapper : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;

        private void Awake()
        {
#if NET_HOST_TEST
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
#elif NET_CLIENT_TEST
            _networkManager.ClientManager.StartConnection();
#endif
        }
    }
}