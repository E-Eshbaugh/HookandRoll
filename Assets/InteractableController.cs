using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isPlayerInRange = false; // Track if player is inside collider

    void Update()
    {
        // Check if player is in range and 'I' is pressed
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.I))
        {
            Interact();
        }
    }

    // When player enters trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure player has the correct tag
        {
            isPlayerInRange = true;
        }
    }

    // When player exits trigger area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // Define what happens when interacting
    private void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Add custom interaction logic here (e.g., open door, pick up item, etc.)
    }
}

