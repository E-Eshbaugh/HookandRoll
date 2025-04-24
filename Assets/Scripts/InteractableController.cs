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
    private InventoryManager inventory;
    private bool givenItem;
    public GameObject itemToGive;
    public  Item checkItem;

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false); // Hide text at the start
        }
        inventory = FindAnyObjectByType<InventoryManager>();
        if (itemToGive == null) {
            itemToGive = Resources.Load<GameObject>("default");
        }
        if (checkItem == null) {
             checkItem = new Item();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.I) && inventory.hasItemofType(checkItem))
        {
            Interact();
            if (isActive) {
                timer += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(timer / timeToFill);
                progressBar.localScale = new Vector3(fillAmount, 0.25f, 1);
                if (timer >= timeToFill && !givenItem) {
                    inventory.AddItemToInventory(itemToGive);
                    givenItem = true;
                }
            }
        } else if (!Input.GetKey(KeyCode.I) && timer > 0f) {
            ResetBar();
        } else if (!inventory.hasItemofType(checkItem)) {
            interactionText.SetActive(true);
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
            givenItem = false;
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
            progressBar.localScale = new Vector2(0, 1);
        }
    }

    private void Interact()
    {
       interact = true;
       
       //SceneManager.LoadScene(sceneToLoad);
    }

    private void ResetBar() {
        timer = 0f;
        progressBar.localScale = new Vector2(0, 1);
        givenItem = false;
    }
}
