using UnityEngine;

namespace XploitCamera
{
    public class FollowController : MonoBehaviour
    {
        public Vector3 targetOffset = new Vector3(0, 1.5f, 0);
        public Vector3 positionOffset = new Vector3(0, -3.2f, 0.55f);
        public float sensitivity = 2f;
        public float maxYAngle = 60f;

        private Vector2 currentRotation;
        private Transform t_target;

        private void Awake()
        {
            //FindPlayer();
        }

        public void FindPlayer(Transform player = null)
        {
            if (player == null)
            {
                t_target = GameObject.FindWithTag("Player").transform;
                Debug.Log("Player finded");
            } else
            {
                t_target = player;
            }
        }

        void Update()
        {
            if (t_target == null) return;

            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

            var rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            transform.position = t_target.position - (rotation * positionOffset);
            transform.LookAt(t_target.position + targetOffset);
        }
    }
}

