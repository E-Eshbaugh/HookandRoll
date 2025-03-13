using UnityEngine;
using TMPro;
public class InteractableController : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public bool playerInRange = false;
    public bool interact = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false); // Hide text at the start
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.I))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure player has the correct tag
        {
            playerInRange = true;
           interactionText.gameObject.SetActive(true);
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionText.gameObject.SetActive(false);
        }
    }

    private void Interact()
    {
       interact = true;
    }
}
