using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class InteractableController : MonoBehaviour
{
    public GameObject interactionText;
    public bool playerInRange = false;
    public bool interact = false;
    public string sceneToLoad;
    
    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false); // Hide text at the start
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
            interactionText.SetActive(true);
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionText.SetActive(false);
        }
    }

    private void Interact()
    {
       interact = true;
       SceneManager.LoadScene(sceneToLoad);
    }
}
