using UnityEngine;
using UnityEngine.UI;

public class WorldInteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask storageLayerMask; // Assign the Layer(s) your Storage objects are on in the Inspector
    [SerializeField] private GameObject storageUIPanel;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape; // Key to close the UI

    public StorageUIManager storageUIManager; // Assign in Inspector or find below

    private InventoryManager inventoryManager; // Reference to InventoryManager

    private Item item; // Reference to the item being interacted with

    private Text dollar;
    void Start()
    {
        // Attempt to find StorageUIManager if not assigned in Inspector
        if (storageUIManager == null && storageUIPanel != null)
        {
            storageUIManager = storageUIPanel.GetComponent<StorageUIManager>();
        }

        // Fallback to searching the scene if still not found
        if (storageUIManager == null)
        {
             storageUIManager = FindObjectOfType<StorageUIManager>(true); // Include inactive
        }


        if (storageUIManager == null)
        {
            Debug.LogError("WorldInteractionManager could not find the StorageUIManager. Assign it in the Inspector or ensure it exists in the scene.", this);
            enabled = false; // Disable this manager if it can't find the UI manager
        }
        else
        {
             Debug.Log($"WorldInteractionManager using StorageUIManager: {storageUIManager.name} (InstanceID: {storageUIManager.GetInstanceID()})", this);
             // Ensure the panel starts inactive
             storageUIManager.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Don't run if UI manager wasn't found
        if (!enabled || storageUIManager == null) return;
        HandleInput();
    }

    private void HandleInput()
    {
        // --- Close UI ---
        // Check if the UI panel itself is active (using the reference)
        if (storageUIManager.gameObject.activeSelf && Input.GetKeyDown(closeKey))
        {
            Debug.Log("Close key pressed."); // DEBUG
            storageUIManager.CloseUI();
            return; // Don't process click if closing
        }

        // Removed the direct panel deactivation here, handled above now.

        // --- Open UI on Click ---
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Debug.Log("Mouse button down detected."); // DEBUG
            // Check if clicking on UI element first - ignore world clicks if UI is hit

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             Debug.Log($"Mouse world position: {mouseWorldPos}");
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, storageLayerMask);
             Debug.Log($"Raycast performed. LayerMask value: {storageLayerMask.value}");

            if (hit.collider != null)
            {
                 Debug.Log($"Raycast hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                Storage clickedStorage = hit.collider.GetComponent<Storage>();
                if (clickedStorage != null)
                {
                    //NEED TO GET ANY FINISHED FOOD FROM INVENTORY
                    inventoryManager = FindObjectOfType<InventoryManager>();
                    if (inventoryManager != null)
                    {
                         Debug.Log($"Found InventoryManager: {inventoryManager.name} (InstanceID: {inventoryManager.GetInstanceID()})", this);
                        // Open the storage UI and pass the clicked
                        item = inventoryManager.hasItemByItemType("Sushi");
                        if (item != null)
                        {
                             inventoryManager.removeItem(item);
                             dollar = GameObject.Find("DollarCounter").GetComponent<Text>();
                             dollar.text = (int.Parse(dollar.text) + item.Value).ToString();
                             Debug.Log($"Added {item.Value} to dollar counter. New value: {dollar.text}");
                             Debug.Log($"Removed item from inventory: {item.name}");
                        }
                        else
                        {
                             Debug.Log("No sushi found in inventory.");
                        }
                    }
                }
                else
                {
                     Debug.Log("Hit object does not have Storage component.");
                    // Clicked somewhere else in the world while UI was open, close it
                    // Clicked somewhere else in the world while UI was open, close it
                    if (storageUIManager.gameObject.activeSelf)
                    {
                         Debug.Log("Closing UI because click was on non-storage object while UI open.");
                         storageUIManager.CloseUI();
                    }
                }
            }
            else
            {
                 Debug.Log("Raycast did not hit any collider on the specified layer mask.");
                 // Clicked empty space while UI was open, close it
                 // Clicked empty space while UI was open, close it
                if (storageUIManager.gameObject.activeSelf)
                {
                     Debug.Log("Closing UI because click was on empty space while UI open.");
                     storageUIManager.CloseUI();
                }
            }
        }
    }
}
