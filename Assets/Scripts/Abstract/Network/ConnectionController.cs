using UnityEngine;
using XploitCamera;

namespace XploitNetwork
{
    public class ConnectionController : MonoBehaviour
    {
        private FollowController _followController;

        private void Awake()
        {
            _followController = Camera.main.GetComponent<FollowController>();
        }

        public void InitSceneAfterConnection()
        {
            _followController.FindPlayer();
        }
    }
}

