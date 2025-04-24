using UnityEngine;

public class CompassHand : MonoBehaviour
{
    // Target position (world origin)
    private Vector3 targetPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
