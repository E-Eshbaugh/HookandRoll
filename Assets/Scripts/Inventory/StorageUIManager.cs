using UnityEngine;
using UnityEngine.UI; // Added for UI elements
using System.Collections.Generic;
// using TMPro; // Uncomment if using TextMeshPro for quantity

public class StorageUIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent Transform for the inventory slots.")]
    public Transform storagePanel;

    [Tooltip("The prefab for a single inventory slot UI element.")]
    public GameObject inventorySlotPrefab;

    private Storage currentOpenStorage; // The storage currently being viewed
    private List<GameObject> currentUISlots = new List<GameObject>();

    // Called by WorldInteractionManager when a storage object is clicked
    public void DisplayStorageContents(Storage storage)
    {
        if (storage == null)
        {
            Debug.LogError("DisplayStorageContents called with null storage.", this);
            return;
        }
        if (storagePanel == null || inventorySlotPrefab == null)
        {
             Debug.LogError("Storage Panel or Inventory Slot Prefab not assigned in the Inspector!", this);
             return;
        }

        currentOpenStorage = storage;
        gameObject.SetActive(true); // Ensure the panel itself is active
        ClearUISlots(); // Clear previous slots

        // Populate UI slots based on storage contents
        foreach (Item item in storage.outputStorage) // Assuming Storage has a public List<Item> outputStorage
        {
            if (item == null) continue; // Skip null items

            GameObject slotGO = Instantiate(inventorySlotPrefab, storagePanel);
            currentUISlots.Add(slotGO);

            // --- Update Slot Visuals ---
            // Get Image component (assuming it's on the prefab or a child)
            Image slotImage = slotGO.GetComponentInChildren<Image>();
            if (slotImage == null)
            {
                Debug.LogWarning($"Inventory Slot Prefab '{inventorySlotPrefab.name}' is missing an Image component.", slotGO);
                continue; // Skip if no image component
            }

            // Get SpriteRenderer from the actual Item GameObject in the world
            SpriteRenderer itemRenderer = item.GetComponent<SpriteRenderer>();
            if (itemRenderer != null && itemRenderer.sprite != null)
            {
                slotImage.sprite = itemRenderer.sprite;
                slotImage.enabled = true; // Make sure image is visible
            }
            else
            {
                slotImage.enabled = false; // Hide image if item has no sprite
                 Debug.LogWarning($"Item '{item.name}' is missing a SpriteRenderer or its sprite is null.", item.gameObject);
            }

            // --- Optional: Update Quantity Text ---
            // TextMeshProUGUI quantityText = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            // if (quantityText != null)
            // {
            //     quantityText.text = item.quantity > 1 ? item.quantity.ToString() : ""; // Show quantity if > 1
            // }
        }
    }

    // Called by WorldInteractionManager to close the UI
    public void CloseUI()
    {
        ClearUISlots();
        currentOpenStorage = null;
        gameObject.SetActive(false); // Deactivate the panel
        Debug.Log("Storage UI Closed"); // DEBUG
    }

    // Helper function to destroy existing UI slots
    private void ClearUISlots()
    {
        foreach (GameObject slot in currentUISlots)
        {
            Destroy(slot);
        }
        currentUISlots.Clear();
    }
}

