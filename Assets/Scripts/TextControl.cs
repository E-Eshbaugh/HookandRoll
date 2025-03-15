using UnityEngine;

public class TextControl : MonoBehaviour
{
    public Transform target;    // Drag the target object here in the Inspector
    public float yOffset = 2f;  // Vertical offset (adjust in Inspector)
    public float xOffset = 2f;
    void Start()
    {
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Follow the target's x-position, but apply y-offset
            Vector3 newPosition = new Vector3(
                target.position.x + xOffset, 
                target.position.y + yOffset, 
                transform.position.z // Maintain original z (for 2D sorting layers)
            );
            transform.position = newPosition;
        }
    }
}
