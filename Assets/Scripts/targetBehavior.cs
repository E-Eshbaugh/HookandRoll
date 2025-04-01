using UnityEngine;

public class targetBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;

    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = 10f;
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        targetPosition.z = 0f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
