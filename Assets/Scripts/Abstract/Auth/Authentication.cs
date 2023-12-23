using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Xploit.HUD;

namespace XploitAuth
{
    public class Authentication : MonoBehaviour
    {
        //private NetworkController _networkController;

        async void Awake()
        {
            //_networkController = GetComponent<NetworkController>();

            try
            {
                await UnityServices.InitializeAsync();
                SetupEvents();
                await SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                string userId = AuthenticationService.Instance.PlayerId;

                Debug.Log($"Sign in anonymously succeeded! PlayerID: {userId}");

                NetworkTestUI.Instance.SetUserId(userId);
            };

            AuthenticationService.Instance.SignInFailed += (err) =>
            {
                Debug.LogError(err);
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
