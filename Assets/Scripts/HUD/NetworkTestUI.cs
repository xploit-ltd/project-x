using Xploit.Abstract.Core.Singleton;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

namespace Xploit.HUD
{
    public class NetworkTestUI : Singleton<NetworkTestUI>
    {
        [SerializeField]
        private Button startServerBtn;

        [SerializeField]
        private Button startClientBtn;

        [SerializeField]
        private TextMeshProUGUI playersInGameText;

        [SerializeField]
        private TMP_InputField joinCode;

        [SerializeField]
        private TextMeshProUGUI uidText;

        private void Awake()
        {
            Cursor.visible = true;
        }

        private void Update()
        {
            playersInGameText.text = $"Players: {PlayersManager.Instance.PlayersInGame}";
        }

        private void Start()
        {
            startServerBtn.onClick.AddListener(async () => {
                if (RelayManager.Instance.IsRelayEnabled)
                {
                    RelayHostData server = await RelayManager.Instance.SetupRelay();
                    joinCode.text = server.JoinCode;
                }

                if (NetworkManager.Singleton.StartHost())
                {
                    Debug.Log($"Host started...");
                }
                else
                {
                    Debug.Log($"Host could not be started...");
                }
            });

            startClientBtn.onClick.AddListener(async () => {
                if (!string.IsNullOrEmpty(joinCode.text) && RelayManager.Instance)
                {
                    RelayJoinData joinData = await RelayManager.Instance.JoinRelay(joinCode.text);
                    Debug.Log("joinData");
                    Debug.Log(joinData);
                }

                if (NetworkManager.Singleton.StartClient())
                {
                    Debug.Log($"Client started...");
                }
                else
                {
                    Debug.Log($"Client could not be started...");
                }
            });
        }

        public void SetUserId(string userId)
        {
            uidText.text = $"UID: {userId}";
        }
    }
}