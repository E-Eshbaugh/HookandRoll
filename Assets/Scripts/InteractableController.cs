using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class InteractableController : MonoBehaviour
{
    public Transform progressBar;
    public float timeToFill = 2f;
    private float timer;
    private bool isActive;
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
        if (isActive) {
            timer += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(timer / timeToFill);
            progressBar.localScale = new Vector3(fillAmount, 0.25f, 1);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure player has the correct tag
        {
            playerInRange = true;
            interactionText.SetActive(true);
            isActive = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionText.SetActive(false);
            isActive = false;
            timer = 0f;
            progressBar.localScale = new Vector3(0, 1, 1);
        }
    }

    private void Interact()
    {
       interact = true;
       //SceneManager.LoadScene(sceneToLoad);
    }
}
