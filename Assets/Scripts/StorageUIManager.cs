using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StorageUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiItemSlotPrefab; // Assign your UI Item Slot prefab (like InventorySlot)
    [SerializeField] private Transform itemContainer; // Assign the child Grid Layout Group Transform (e.g., ItemGrid)

    [Header("Layout Settings")]
    [SerializeField] private int desiredColumnCount = 3; // How many columns the grid should have
    [SerializeField] private Vector2 cellSize = new Vector2(50f, 50f); // Size of each slot
    [SerializeField] private Vector2 spacing = new Vector2(2.5f, 2.5f); // Space between slots
    [SerializeField] private RectOffset padding = new RectOffset(0, 0, 0, 0); // Padding around the grid

    private Storage currentOpenStorage; // The storage currently being viewed
    private List<GameObject> currentUISlots = new List<GameObject>();
    private RectTransform panelRectTransform;
    private GridLayoutGroup gridLayoutGroup;

    // OnEnable is no longer needed to automatically update UI.
    // The panel should start disabled in the prefab.

    // Removed Update() and HandleInput() - Logic moved to WorldInteractionManager

    // This method is now called by WorldInteractionManager
    public void OpenUIForStorage(Storage storage)
    {
         Debug.Log($"StorageUIManager: OpenUIForStorage called for {(storage != null ? storage.name : "NULL")}. Current state: {gameObject.activeSelf}", this); // DEBUG
        if (storage == null)
        {
            Debug.LogWarning("OpenUIForStorage called with null storage.", this);
            return;
        }

        currentOpenStorage = storage;
         Debug.Log($"StorageUIManager: Activating GameObject. Parent active? {transform.parent?.gameObject.activeInHierarchy ?? true}", this); // DEBUG
        gameObject.SetActive(true); // Activate this panel
         Debug.Log($"StorageUIManager: GameObject activeSelf after SetActive(true): {gameObject.activeSelf}", this); // DEBUG
        UpdateUI(); // Populate with items from the newly opened storage
        // Debug log moved earlier
    }

    public void CloseUI()
    {
        currentOpenStorage = null;
        gameObject.SetActive(false); // Deactivate this panel
        Debug.Log("StorageUIManager: Closed UI");
    }

    void Awake()
    {
        // Get components on Awake
        panelRectTransform = GetComponent<RectTransform>();
        // IMPORTANT: This assumes the GridLayoutGroup is on the SAME GameObject as StorageUIManager.
        // If itemContainer is a child, you might need gridLayoutGroup = itemContainer.GetComponent<GridLayoutGroup>();
        // Let's assume it's on the same object for now based on previous findings.
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (panelRectTransform == null)
        {
            Debug.LogError("StorageUIManager requires a RectTransform component on the same GameObject.", this);
            enabled = false;
        }
        if (gridLayoutGroup == null)
        {
             // Check if it's on the itemContainer instead
             if (itemContainer != null) {
                 gridLayoutGroup = itemContainer.GetComponent<GridLayoutGroup>();
             }

             if (gridLayoutGroup == null) {
                Debug.LogError("StorageUIManager requires a GridLayoutGroup component, either on the same GameObject or on the assigned Item Container.", this);
                enabled = false;
             }
        }
         if (itemContainer == null) {
             Debug.LogError("StorageUIManager requires the Item Container transform to be assigned in the Inspector.", this);
             enabled = false;
         }
    }

    // Call this method to refresh the UI display based on the Storage content
    public void UpdateUI()
    {
        // Check if a storage is actually open and references are set
        if (currentOpenStorage == null)
        {
             Debug.LogWarning("UpdateUI called but no storage is currently open.", this);
             // Optionally clear UI if needed, or just return
             ClearUI();
             return;
        }

        // Ensure required components are present (check gridLayoutGroup again in case it was found in Awake on itemContainer)
         if (gridLayoutGroup == null && itemContainer != null) {
             gridLayoutGroup = itemContainer.GetComponent<GridLayoutGroup>();
         }

        if (uiItemSlotPrefab == null || itemContainer == null || panelRectTransform == null || gridLayoutGroup == null)
        {
            Debug.LogError("StorageUIManager is missing references (Slot Prefab, Item Container, RectTransform, or GridLayoutGroup). Check Inspector and component setup.", this);
            CloseUI(); // Close UI if misconfigured
            return;
        }

        // --- Resize Panel and Configure Grid ---
        int slotCount = currentOpenStorage.maxOutputCapacity;
        if (slotCount <= 0) slotCount = 1; // Prevent division by zero if capacity is 0
        if (desiredColumnCount <= 0) desiredColumnCount = 1; // Prevent division by zero

        int numberOfRows = Mathf.CeilToInt((float)slotCount / desiredColumnCount);

        // Calculate required size
        float totalWidth = padding.left + padding.right + (desiredColumnCount * cellSize.x) + (Mathf.Max(0, desiredColumnCount - 1) * spacing.x);
        float totalHeight = padding.top + padding.bottom + (numberOfRows * cellSize.y) + (Mathf.Max(0, numberOfRows - 1) * spacing.y);

        // Apply size to RectTransform of the panel this script is on
        panelRectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Configure GridLayoutGroup (assuming it's on the itemContainer or this object)
        gridLayoutGroup.padding = padding;
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spacing;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = desiredColumnCount;

        // --- Clear and Populate Slots ---
        // Make sure slots are parented to the correct container (assigned in Inspector)
        ClearUI(); // ClearUI already handles destroying children of itemContainer if setup correctly

        // Populate the UI based on the storage size
        for (int i = 0; i < currentOpenStorage.maxOutputCapacity; i++) // Iterate based on capacity (using maxOutputCapacity)
        {
            GameObject newSlot = Instantiate(uiItemSlotPrefab, itemContainer);
            currentUISlots.Add(newSlot);
            Image slotImage = newSlot.GetComponent<Image>();

            if (slotImage == null)
            {
                Debug.LogError("InventorySlot prefab is missing an Image component on its root.", uiItemSlotPrefab);
                continue; // Skip this slot if prefab is misconfigured
            }

            // Check if an item exists at this index in the storage
            if (i < currentOpenStorage.outputStorage.Count)
            {
                Item item = currentOpenStorage.outputStorage[i];
                if (item != null) // Check if the item itself is not null
                {
                    SpriteRenderer itemSpriteRenderer = item.GetComponent<SpriteRenderer>();

                    if (itemSpriteRenderer != null && itemSpriteRenderer.sprite != null)
                    {
                        // Assign the item's sprite to the UI slot's image
                        slotImage.sprite = itemSpriteRenderer.sprite;
                        slotImage.enabled = true; // Ensure the image is visible
                    }
                    else
                    {
                        // Item exists but has no sprite, show empty slot
                        slotImage.sprite = null;
                        slotImage.enabled = false;
                        if (itemSpriteRenderer == null)
                        {
                            Debug.LogWarning($"Item '{item.name}' at index {i} is missing a SpriteRenderer.", item);
                        }
                        else // itemSpriteRenderer exists but sprite is null
                        {
                             Debug.LogWarning($"Item '{item.name}' at index {i} has a SpriteRenderer but no sprite assigned.", item);
                        }
                    }
                }
                else
                {
                     // Null item entry in the list, treat as empty slot
                     slotImage.sprite = null;
                     slotImage.enabled = false;
                     Debug.LogWarning($"Null item found at index {i} in storage '{currentOpenStorage.name}'.", currentOpenStorage);
                }
            }
            else
            {
                // No item at this index (index >= count), show empty slot
                slotImage.sprite = null;
                slotImage.enabled = false;
            }

            // TODO: Add logic here if you need to handle clicking/dragging items from the storage UI.
        }
    }

    private void ClearUI()
    {
        // Clear existing UI slots
        foreach (GameObject slot in currentUISlots)
        {
            // Use DestroyImmediate if modifying prefab directly, otherwise Destroy is fine for scene instances
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(slot);
            else
                Destroy(slot);
        }
        currentUISlots.Clear();
    }

    // TEMPORARY FIX for compile error CS1061 - Remove ToggleStorageUIOnClick script from storage objects!
    public void SetTargetStorage(Storage storage)
    {
        // This method is intentionally left empty.
        // It only exists to prevent the compile error from the old ToggleStorageUIOnClick script.
        // The actual logic is now handled by OpenUIForStorage.
        Debug.LogWarning("SetTargetStorage called (temporary fix). Please remove ToggleStorageUIOnClick script from storage objects.", this);
    }
}