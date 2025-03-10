using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }

    [SerializeField] private GameObject hotbarPanel;
    [SerializeField] private Transform hotbarSlotsParent;
    [SerializeField] private GameObject hotbarSlotPrefab;
    [SerializeField] private int hotbarSize = 10;
    [SerializeField] private Image selectionHighlight;
    
    private List<HotbarSlot> hotbarSlots = new List<HotbarSlot>();
    private int selectedSlotIndex = 0;
    
    [SerializeField] private PlacingMode placingMode;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeHotbar();
        UpdateSelectionHighlight();
    }

    private void Update()
    {
        // Number key selection (1-0)
        for (int i = 0; i < Mathf.Min(hotbarSize, 10); i++)
        {
            // For keys 1-9
            if (i < 9 && Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetSelectedSlot(i);
                break;
            }
            // For key 0 (10th slot)
            else if (i == 9 && Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetSelectedSlot(9);
                break;
            }
        }
        
        // Mouse wheel scrolling
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            int newIndex;
            if (scrollDelta > 0)
            {
                // Scroll up - move left in hotbar
                newIndex = (selectedSlotIndex - 1 + hotbarSize) % hotbarSize;
            }
            else
            {
                // Scroll down - move right in hotbar
                newIndex = (selectedSlotIndex + 1) % hotbarSize;
            }
            SetSelectedSlot(newIndex);
        }
        
        // Hotkey to use/place selected item
        if (Input.GetMouseButtonDown(0))
        {
            // Attempt to place or use the selected item
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // For 2D
            
            UseSelectedItem(mousePos);
        }
    }

    private void InitializeHotbar()
    {
        // Create hotbar slots
        for (int i = 0; i < hotbarSize; i++)
        {
            GameObject slotObj = Instantiate(hotbarSlotPrefab, hotbarSlotsParent);
            HotbarSlot slot = slotObj.GetComponent<HotbarSlot>();
            slot.SetIndex(i);
            hotbarSlots.Add(slot);
        }
    }

    public void SetSelectedSlot(int index)
    {
        if (index >= 0 && index < hotbarSize)
        {
            selectedSlotIndex = index;
            UpdateSelectionHighlight();
            
            Item selectedItem = GetSelectedItem();
            
            // Change the placement mode based on selected item
            if (selectedItem != null && placingMode != null)
            {
                switch (selectedItem.id)
                {
                    // IDs for placeable structures like conveyors, storage, etc.
                    case 1: // Conveyor belt
                        placingMode.SetPlacingType(PlacingMode.PlacingType.ConveyorBelt);
                        break;
                    case 2: // Storage
                        placingMode.SetPlacingType(PlacingMode.PlacingType.Storage);
                        break;
                    case 3: // Resource extractor
                        placingMode.SetPlacingType(PlacingMode.PlacingType.Extractor);
                        break;
                    default:
                        // Regular items - might be placeable on conveyors
                        placingMode.SetPlacingType(PlacingMode.PlacingType.None);
                        break;
                }
            }
            else if (placingMode != null)
            {
                // Nothing selected
                placingMode.SetPlacingType(PlacingMode.PlacingType.None);
            }
        }
    }

    private void UpdateSelectionHighlight()
    {
        if (selectionHighlight != null && selectedSlotIndex < hotbarSlots.Count)
        {
            RectTransform slotRect = hotbarSlots[selectedSlotIndex].GetComponent<RectTransform>();
            selectionHighlight.rectTransform.position = slotRect.position;
        }
    }

    public Item GetSelectedItem()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < hotbarSlots.Count)
        {
            return hotbarSlots[selectedSlotIndex].GetItem();
        }
        return null;
    }
    
    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }
    
    public bool UseSelectedItem(Vector3 worldPosition)
    {
        Item selectedItem = GetSelectedItem();
        if (selectedItem == null) return false;
        
        // Reference to inventory slot that contains this item
        int inventorySlotIndex = hotbarSlots[selectedSlotIndex].GetLinkedInventorySlot();
        if (inventorySlotIndex < 0) return false;
        
        // For automation game - handle item usage or placement
        Grid grid = FindFirstObjectByType<Grid>();
        if (grid != null)
        {
            Vector3Int cellPos = grid.WorldToCell(worldPosition);
            
            // Check if the location is valid for placement
            bool canPlace = true;
            IPlaceable existingPlaceable = GridManager.Instance.GetPlaceableAt(cellPos);
            if (existingPlaceable != null)
            {
                canPlace = false;
                
                // Special case: placing items on conveyor belts
                if (existingPlaceable is ConveyorBeltTile conveyor && conveyor.CurrentItem == null)
                {
                    // Place the item on the conveyor
                    GameObject itemObj = InventoryManager.Instance.CreateItemGameObject(selectedItem);
                    conveyor.ReceiveItem(itemObj);
                    
                    // Remove from inventory
                    InventoryManager.Instance.RemoveItem(inventorySlotIndex, 1);
                    return true;
                }
            }
            
            if (canPlace)
            {
                // Handle placement of different structures based on the item type
                // Here you would call into your existing placement system
                
                // Example for conveyor belt placement:
                if (selectedItem.id == 1) // Assuming ID 1 is conveyor belt
                {
                    // Create and place conveyor
                    GameObject conveyorPrefab = Resources.Load<GameObject>("Prefabs/ConveyorBelt");
                    if (conveyorPrefab != null)
                    {
                        GameObject conveyor = Instantiate(conveyorPrefab, grid.GetCellCenterWorld(cellPos), Quaternion.identity);
                        ConveyorBeltTile conveyorTile = conveyor.GetComponent<ConveyorBeltTile>();
                        if (conveyorTile != null)
                        {
                            // Remove from inventory
                            InventoryManager.Instance.RemoveItem(inventorySlotIndex, 1);
                            return true;
                        }
                    }
                }
                
                // Add cases for other structure types
            }
        }
        
        return false;
    }
    
    // For placing items in the hotbar from the inventory
    public void SetHotbarItem(int hotbarIndex, int inventoryIndex)
    {
        if (hotbarIndex >= 0 && hotbarIndex < hotbarSlots.Count)
        {
            Item item = InventoryManager.Instance.GetItem(inventoryIndex);
            hotbarSlots[hotbarIndex].SetLinkedInventorySlot(inventoryIndex);
            hotbarSlots[hotbarIndex].SetItem(item);
        }
    }
}

// Enum for your placing mode - you'll need to create this class or adapt to your system
public class PlacingMode : MonoBehaviour
{
    public enum PlacingType
    {
        None,
        ConveyorBelt,
        Storage,
        Extractor,
        Factory
    }
    
    private PlacingType currentPlacingType = PlacingType.None;
    
    public void SetPlacingType(PlacingType type)
    {
        currentPlacingType = type;
        // Handle UI updates or other logic
    }
    
    // Additional placement logic would go here
}