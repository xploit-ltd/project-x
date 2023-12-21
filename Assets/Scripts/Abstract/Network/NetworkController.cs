using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace XploitNetwork
{
    public class NetworkController : MonoBehaviour
    {
        private GameObject _system;
        private ConnectionController _connectionController;

        private void Awake()
        {
            _system = GameObject.FindWithTag("XploitSystem");
            if (_system)
            {
                _connectionController = _system.GetComponent<ConnectionController>();
            }
        }

        /// <summary>
        /// Starts a game host with a relay allocation: it initializes the Unity services, signs in anonymously and starts the host with a new relay allocation.
        /// </summary>
        /// <param name="maxConnections">Maximum number of connections to the created relay.</param>
        /// <returns>The join code that a client can use.</returns>
        /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
        /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
        /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
        /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
        /// <exception cref="RequestFailedException"> See <see cref="IAuthenticationService.SignInAnonymouslyAsync"/></exception>
        /// <exception cref="ArgumentException">Thrown when the maxConnections argument fails validation in Relay Service SDK.</exception>
        /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>

        public async Task<string> StartHostWithRelay(int maxConnections = 5)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                SetupEvents();

                Debug.Log($"joinCode={joinCode}");

                return NetworkManager.Singleton.StartHost() ? joinCode : null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Joins a game with relay: it will initialize the Unity services, sign in anonymously, join the relay with the given join code and start the client.
        /// </summary>
        /// <param name="joinCode">The join code of the allocation</param>
        /// <returns>True if starting the client was successful</returns>
        /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
        /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
        /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
        /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
        /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation service.</exception>
        /// <exception cref="ArgumentException">Thrown if the joinCode has the wrong format.</exception>
        /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>
        public async Task<bool> StartClientWithRelay(string joinCode)
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            SetupEvents();

            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }

        public void SetupEvents()
        {
            //NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        }

        public void OnClientStarted()
        {
            Debug.Log($"OnClientStarted");

            _connectionController.InitSceneAfterConnection();
        }
    }
}

