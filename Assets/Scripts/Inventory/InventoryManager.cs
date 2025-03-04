using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventorySlotsParent;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private int inventorySize = 24;

    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private List<Item> inventoryItems = new List<Item>();

    // Reference to player interaction system that handles placement
    [SerializeField] private PlayerInteraction playerInteraction;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keeps the manager between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Validate required references
        if (inventorySlotsParent == null)
        {
            Debug.LogError("Inventory slots parent not assigned in InventoryManager!");
            // Try to find it in this object's children
            inventorySlotsParent = transform.Find("InventoryPanel/InventoryGrid");
            if (inventorySlotsParent != null)
                Debug.Log("Found inventory slots parent via search");
        }
        
        if (inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory slot prefab not assigned in InventoryManager!");
        }
        
        // Clear the list first to ensure we don't have leftovers
        inventoryItems.Clear();
        
        // Initialize inventoryItems list with nulls
        for (int i = 0; i < inventorySize; i++)
        {
            inventoryItems.Add(null);
        }
        
        Debug.Log($"Inventory initialized with {inventorySize} slots");
    }

    private void Start()
    {
        InitializeInventory();
        
        // Initially hide the inventory panel
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle inventory with 'I' key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void InitializeInventory()
    {
        // Log inventory UI components for debugging
        Debug.Log($"Initializing Inventory - Panel: {(inventoryPanel != null ? "Found" : "Missing")}, " +
                  $"SlotsParent: {(inventorySlotsParent != null ? "Found" : "Missing")}, " +
                  $"SlotPrefab: {(inventorySlotPrefab != null ? "Found" : "Missing")}");
        
        // Ensure we have the required references
        if (inventorySlotsParent == null || inventorySlotPrefab == null)
        {
            Debug.LogError("Missing critical references for inventory initialization!");
            return;
        }
        
        // Initialize inventory with empty slots
        for (int i = 0; i < inventorySize; i++)
        {
            if (i >= inventoryItems.Count)
            {
                inventoryItems.Add(null);
            }

            GameObject slotObj = Instantiate(inventorySlotPrefab, inventorySlotsParent);
            slotObj.name = $"InventorySlot_{i}";  // Give a unique name for easier debugging
            
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot == null)
            {
                Debug.LogError($"Inventory slot prefab does not have InventorySlot component!");
                continue;
            }
            
            slot.SetIndex(i);
            inventorySlots.Add(slot);
            
            // Update UI for any items that were added before UI initialization
            if (inventoryItems[i] != null)
            {
                Debug.Log($"Setting item {inventoryItems[i].itemName} to slot {i} during initialization");
                slot.SetItem(inventoryItems[i]);
            }
        }
        
        // After initialization, refresh the UI
        RefreshInventoryUI();
        Debug.Log($"Inventory initialized with {inventorySlots.Count} slots");
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);
            
            Debug.Log($"Toggling inventory: {(isActive ? "OPEN" : "CLOSED")}");
            
            // When opening inventory, refresh its contents
            if (isActive)
            {
                // Make sure all slots in the inventory are active and visible
                for (int i = 0; i < inventorySlots.Count; i++) 
                {
                    if (inventorySlots[i] != null && inventorySlots[i].gameObject != null)
                    {
                        inventorySlots[i].gameObject.SetActive(true);
                    }
                }
                
                // Force inventory panel to be visible
                Canvas canvas = inventoryPanel.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvas.enabled = true;
                }
                
                RefreshInventoryUI();
                
                Debug.Log($"Inventory refreshed with {inventoryItems.Count} item slots");
                // Log first few items for debugging
                for (int i = 0; i < inventoryItems.Count && i < 5; i++)
                {
                    if (inventoryItems[i] != null)
                    {
                        Debug.Log($"Slot {i}: {inventoryItems[i].itemName} (Icon: {(inventoryItems[i].icon != null ? inventoryItems[i].icon.name : "None")})");
                    }
                    else
                    {
                        Debug.Log($"Slot {i}: Empty");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Inventory panel reference is missing!");
        }
    }

    public void RefreshInventoryUI()
    {
        // Update all slots to reflect current inventory state
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventoryItems.Count && inventoryItems[i] != null)
            {
                inventorySlots[i].SetItem(inventoryItems[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    // Add an item to the inventory
    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd == null) return false;

        // Try to stack with existing items first
        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i] != null && inventoryItems[i].id == itemToAdd.id)
                {
                    inventoryItems[i].AddQuantity(itemToAdd.quantity);
                    RefreshInventoryUI();
                    return true;
                }
            }
        }

        // Make sure inventorySlots is populated before checking if slots are empty
        if (inventorySlots.Count > 0)
        {
            // Find first empty slot
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i] == null)
                {
                    inventoryItems[i] = itemToAdd.Clone(); // Create a copy to avoid reference issues
                    
                    // Update the UI slot if it exists
                    if (i < inventorySlots.Count && inventorySlots[i] != null)
                    {
                        inventorySlots[i].SetItem(inventoryItems[i]);
                    }
                    
                    RefreshInventoryUI();
                    return true;
                }
            }
        }
        else
        {
            // If UI slots aren't initialized yet, just add to the data structure
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i] == null)
                {
                    inventoryItems[i] = itemToAdd.Clone();
                    return true;
                }
            }
        }

        // Inventory is full
        Debug.Log("Inventory full! Couldn't add " + itemToAdd.itemName);
        return false;
    }

    // Remove an item from the inventory
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count || inventoryItems[slotIndex] == null)
        {
            return false;
        }

        Item item = inventoryItems[slotIndex];
        item.RemoveQuantity(quantity);

        if (item.quantity <= 0)
        {
            inventoryItems[slotIndex] = null;
        }

        RefreshInventoryUI();
        return true;
    }

    // Get an item without removing it (used by hotbar)
    public Item GetItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventoryItems.Count)
        {
            return inventoryItems[slotIndex];
        }
        return null;
    }
    
    // For your automation game - convert an Item to a GameObject with ItemComponent
    public GameObject CreateItemGameObject(Item item)
    {
        if (item == null) return null;
        
        GameObject itemObject = new GameObject("Item_" + item.itemName);
        ItemComponent itemComponent = itemObject.AddComponent<ItemComponent>();
        itemComponent.item = item.Clone();
        
        // Add a sprite renderer for visual representation on conveyors
        SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
        if (item.icon != null)
        {
            spriteRenderer.sprite = item.icon;
        }
        
        // Add collider for physics interactions if needed
        itemObject.AddComponent<BoxCollider2D>();
        
        return itemObject;
    }
    
    // For use with your automation system - place item from inventory into the world
    public bool PlaceItemInWorld(int slotIndex, Vector3 worldPosition)
    {
        Item item = GetItem(slotIndex);
        if (item == null || item.quantity <= 0) return false;
        
        // For placeable building items like conveyors, storage, etc.
        // You would call your placement system here
        
        // For regular items that go on conveyors
        GameObject itemObject = CreateItemGameObject(item);
        itemObject.transform.position = worldPosition;
        
        // Remove one from inventory
        RemoveItem(slotIndex, 1);
        
        return true;
    }

    public int GetInventorySize()
    {
        return inventorySize;
    }
    
    public void ClearInventory()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItems[i] = null;
        }
        RefreshInventoryUI();
        Debug.Log("Inventory cleared");
    }
}