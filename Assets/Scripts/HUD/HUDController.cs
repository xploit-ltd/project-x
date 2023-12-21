using TMPro;
using UnityEngine;
using XploitNetwork;

namespace XploitHUD
{
    public class HUDController : MonoBehaviour
    {
        public GameObject topArea;
        public string joinCode;
        public TMP_InputField codeInput;

        private GameObject _system;
        private NetworkController _networkController;

        private void Awake()
        {
            _system = GameObject.FindWithTag("XploitSystem");
            _networkController = _system.GetComponent<NetworkController>();
        }

        void Start()
        {
            HideTopArea();
        }

        public void HideTopArea()
        {
            topArea.SetActive(false);
        }

        public void ShowTopArea()
        {
            topArea.SetActive(true);
        }

        public async void CreateServer()
        {
            joinCode = await _networkController.StartHostWithRelay();
            codeInput.text = joinCode;
        }

        public async void ConnectServer()
        {
            joinCode = codeInput.text;
            await _networkController.StartClientWithRelay(joinCode);
        }
    }
}
