using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using XploitHUD;
using XploitNetwork;

namespace XploitAuth
{
    public class Authentication : MonoBehaviour
    {
        //private NetworkController _networkController;
        private GameObject _hud;
        private HUDController _hudController;

        async void Awake()
        {
            _hud = GameObject.FindWithTag("XploitHUD");
            _hudController = _hud.GetComponent<HUDController>();
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
                Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");

                _hudController.ShowTopArea();
            };

            AuthenticationService.Instance.SignInFailed += (err) =>
            {
                Debug.LogError(err);

                _hudController.HideTopArea();
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Player signed out.");

                _hudController.HideTopArea();
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");

                _hudController.HideTopArea();
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
