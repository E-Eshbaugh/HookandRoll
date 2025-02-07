using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero; 
    public Vector3 offset;
    
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

    // camera without smoothing
    // void LateUpdate()
    // {
    //     if (target != null)
    //     {
    //         // Update camera position to follow target
    //         Vector3 newPosition = new Vector3(target.position.x + offset.x, 
    //                                           target.position.y + offset.y, 
    //                                           transform.position.z + offset.z);
    //         transform.position = newPosition;
    //     }
    // }
}
