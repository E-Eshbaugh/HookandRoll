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
    public Transform inventory;
    public Item itemToGive;
    private bool givenItem;

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
            if (timer >= timeToFill && !givenItem) {
                if (itemToGive != null) {
                    giveItem(itemToGive);
                } else {
                    Debug.Log("No item to give");
                    givenItem = true;
                }
               
            }
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
            progressBar.localScale = new Vector3(0, 1, 1);
        }
    }

    private void Interact()
    {
       interact = true;
       //SceneManager.LoadScene(sceneToLoad);
    }

    private void giveItem(Item item) {
        foreach (Transform slot in inventory) {
            if (slot.childCount == 0 && !givenItem) {
                Item newItem = Instantiate(item, slot);
                newItem.transform.localPosition = Vector2.zero;

                Item itemScript = newItem.GetComponent<Item>();
                if (itemScript != null) {
                    itemScript.parentAfterDrag = slot;
                }
                givenItem = true;
            }
        }
        Debug.LogWarning("No empty toolbar slots!");
    }
}
