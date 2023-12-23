using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Xploit.HUD
{
    public class PlayerHUD : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI playerNameField;

        private NetworkVariable<NetworkString> playersName = new();
        private bool overlaySet = false;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                playersName.Value = $"Player {OwnerClientId}";
            }
        }

        public void SetOverlay()
        {
            playerNameField.text = playersName.Value;
        }

        private void Update()
        {
            if (!overlaySet && !string.IsNullOrEmpty(playersName.Value))
            {
                SetOverlay();
                overlaySet = true;
            }
        }
    };

}