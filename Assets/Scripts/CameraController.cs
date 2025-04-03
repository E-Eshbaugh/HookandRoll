using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero; 
    public Vector3 offset;
    public CinemachineCamera vcam;
    public Transform player;
    public Transform boat;
    public bool inBoat = false;


    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            // use smoothdamp because it includes interpolation so no jittering
            Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
            smoothPos.z = transform.position.z; // preserve the camera's Z value
            transform.position = smoothPos;
        }
    }

    void Update()
    {
        vcam.Follow = inBoat ? boat : player;
    }
}
