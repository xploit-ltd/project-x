using UnityEngine;

public class FollowController : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0,0,0);
    public Vector3 positionOffset = new Vector3(0, -4.5f, 2.3f);
    public float sensitivity = 2f;
    public float maxYAngle = 60f;

    private Vector2 currentRotation;

    void Update()
    {
        if (target != null)
        {
            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

            var rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            transform.position = target.position - (rotation * positionOffset);
            transform.LookAt(target.position + targetOffset);
        }
    }
}
