using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;


public class InteractableController : MonoBehaviour
{
    public Transform progressBar;
    public float timeToFill = 2f;
    private float timer;
    private bool isActive;
    public Text interactionText;
    public bool playerInRange = false;
    public bool interact = false;
    public string sceneToLoad;
    private InventoryManager inventory;
    private bool givenItem;
    public List<ItemRecipe> ReturnItems = new List<ItemRecipe>();
    public GameObject self;


    void Start()
    {

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        inventory = FindAnyObjectByType<InventoryManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.I))
        {
            Interact();
            if (isActive) {
                timer += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(timer / timeToFill);
                progressBar.localScale = new Vector3(fillAmount, 0.25f, 1);
                if (timer >= timeToFill && !givenItem) {
                    ProcessItem();
            }
        }
    } else if (!Input.GetKey(KeyCode.I) && timer > 0f) {
        ResetBar();
    } 
        
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (self.CompareTag("Stove")) {
            interactionText.text = "Hold I to Cook Rice";
        } else if (self.CompareTag("Fridge")) {
            interactionText.text = "Press I to Open Fridge";
        } else if (self.CompareTag("Crates")) {
            interactionText.text = "Hold I to Buy Rice";
        } else if (self.CompareTag("RollingStation")) {
            interactionText.text = "Hold I to Roll";
        } else if (self.CompareTag("CuttingStation")) {
            interactionText.text = "Hold I to Cut";
        } else {
            interactionText.text = "Press I to Interact";
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionText.gameObject.SetActive(true);
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
            interactionText.gameObject.SetActive(false);
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

    private void ResetBar() {
        timer = 0f;
        progressBar.localScale = new Vector3(0, 0.25f, 1);
        givenItem = false;
    }

    private void ProcessItem() {
        foreach (var recipe in ReturnItems) {
            if (recipe == null) {
                continue;
            }
            bool hasRequiredItems = true;
            foreach (var item in recipe.requiredItems){
                if (!inventory.hasItem(item)) {
                    hasRequiredItems = false;
                    break;
                }
            }
            if (hasRequiredItems) {
                foreach (var requiredItems in recipe.requiredItems) {
                    inventory.removeItem(requiredItems);
                }
                inventory.AddItemToInventory(recipe.returnPrefab);
                givenItem = true;
                return;
            }
        }
        
        ResetBar();
    }
}
