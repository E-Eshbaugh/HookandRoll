using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Depot : PlaceableBase {
    public ConveyorBeltTile InputBelt;
    public ConveyorBeltTile OutputBelt;
    public int Capacity = 10;
    public List<Item> Items = new List<Item>();
    public GameObject ItemPrefab;
    
    // UI References
    [SerializeField] private GameObject depotPanel;
    [SerializeField] private Transform depotSlotParent;
    [SerializeField] private GameObject depotSlotPrefab;
    [SerializeField] private TextMeshProUGUI depotTitleText;
    [SerializeField] private Image depotIcon;
    
    // Tracking UI state
    private bool isUIOpen = false;
    private List<DepotSlot> depotSlots = new List<DepotSlot>();
    
    protected override void Awake() {
        base.Awake();
        
        // Find UI references if not set in inspector
        if (depotPanel == null) {
            // Try to find in scene by tag or name
            depotPanel = GameObject.FindWithTag("DepotPanel");
        }
    }
    
    protected override void Start() {
        base.Start();
        SetupConveyorConnections();
        
        // Ensure the depot panel is hidden on start
        if (depotPanel != null) {
            depotPanel.SetActive(false);
        }
        
        // Initialize the depot UI slots
        InitializeDepotUI();
    }
    
    private void SetupConveyorConnections() {
        // Output is in the same direction as the depot's Direction.
        Vector3Int outputOffset = Direction;
        // Input comes from the opposite direction.
        Vector3Int inputOffset = -Direction;
        
        IPlaceable potentialInput = GridManager.Instance.GetPlaceableAt(GridPosition + inputOffset);
        IPlaceable potentialOutput = GridManager.Instance.GetPlaceableAt(GridPosition + outputOffset);
        
        if (potentialInput is ConveyorBeltTile inTile) {
            InputBelt = inTile;
        }
        
        if (potentialOutput is ConveyorBeltTile outTile) {
            OutputBelt = outTile;
        }
    }
    
    // Initialize the depot UI slots for showing inventory
    private void InitializeDepotUI() {
        if (depotSlotParent != null && depotSlotPrefab != null) {
            // Clear any existing slots
            foreach (Transform child in depotSlotParent) {
                Destroy(child.gameObject);
            }
            depotSlots.Clear();
            
            // Create slots based on capacity
            for (int i = 0; i < Capacity; i++) {
                GameObject slotObj = Instantiate(depotSlotPrefab, depotSlotParent);
                DepotSlot depotSlot = slotObj.GetComponent<DepotSlot>();
                if (depotSlot != null) {
                    depotSlot.SetIndex(i);
                    depotSlot.SetDepot(this);
                    depotSlots.Add(depotSlot);
                }
            }
        }
    }
    
    // Update the UI to match the current depot state
    public void RefreshDepotUI() {
        if (depotSlots.Count == 0) return;
        
        // Update each slot with its corresponding item
        for (int i = 0; i < depotSlots.Count; i++) {
            if (i < Items.Count && Items[i] != null) {
                depotSlots[i].SetItem(Items[i]);
            } else {
                depotSlots[i].ClearSlot();
            }
        }
        
        // Update title text if available
        if (depotTitleText != null) {
            depotTitleText.text = $"Depot ({Items.Count}/{Capacity})";
        }
    }
    
    // Toggle the depot UI visibility
    public void ToggleDepotUI() {
        if (depotPanel != null) {
            isUIOpen = !isUIOpen;
            depotPanel.SetActive(isUIOpen);
            
            if (isUIOpen) {
                // Pause game or enter "UI mode" if needed
                Time.timeScale = 0f; // Optional - pause the game while UI is open
                
                RefreshDepotUI();
            } else {
                // Resume game
                Time.timeScale = 1f;
            }
        }
    }
    
    // Player interaction with depot
    public void OnPlayerInteract() {
        ToggleDepotUI();
    }
    
    // Automatically push an item onto the output conveyor on each tick.
    protected override void ProcessTick() {
        // Only process automation if UI is not open
        if (!isUIOpen) {
            PushItem();
        }
    }
    
    public bool AddItem(Item item) {
        if (Items.Count < Capacity) {
            Items.Add(item);
            Debug.Log("Added item: " + item.itemName);
            
            // Update UI if it's open
            if (isUIOpen) {
                RefreshDepotUI();
            }
            
            return true;
        }
        
        Debug.LogWarning("Depot storage is full.");
        return false;
    }
    
    public bool RemoveItem(int index) {
        if (index >= 0 && index < Items.Count) {
            Item item = Items[index];
            Items.RemoveAt(index);
            Debug.Log("Removed item: " + item.itemName);
            
            // Update UI if it's open
            if (isUIOpen) {
                RefreshDepotUI();
            }
            
            return true;
        }
        
        Debug.LogWarning("Item not found in depot.");
        return false;
    }
    
    public Item GetItem(int index) {
        if (index >= 0 && index < Items.Count) {
            return Items[index];
        }
        return null;
    }
    
    public void PushItem() {
        if (Items.Count > 0 && OutputBelt != null && OutputBelt.CurrentItem == null) {
            GameObject itemObj = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
            ItemComponent itemComp = itemObj.GetComponent<ItemComponent>();
            itemComp.item = Items[0];
            OutputBelt.ReceiveItem(itemObj);
            Items.RemoveAt(0);
            
            // Update UI if it's open
            if (isUIOpen) {
                RefreshDepotUI();
            }
        }
    }
    
    public void PullItem() {
        if (InputBelt != null && InputBelt.CurrentItem != null) {
            GameObject pulledItem = InputBelt.CurrentItem;
            // Extract the ItemComponent that holds the underlying item data.
            ItemComponent itemComp = pulledItem.GetComponent<ItemComponent>();
            
            if (itemComp != null && itemComp.item != null) {
                if (AddItem(itemComp.item)) {
                    Destroy(pulledItem);
                    InputBelt.CurrentItem = null;
                }
            }
        }
    }
    
    // Try to transfer an item from player inventory to depot
    public bool TransferFromInventory(Item item) {
        return AddItem(item);
    }
    
    // Try to transfer an item from depot to player inventory
    public bool TransferToInventory(int depotSlotIndex) {
        Item item = GetItem(depotSlotIndex);
        
        if (item != null) {
            bool added = InventoryManager.Instance.AddItem(item);
            
            if (added) {
                RemoveItem(depotSlotIndex);
                return true;
            }
        }
        
        return false;
    }
}

// UI Slot for the Depot inventory
public class DepotSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler {
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject quantityPanel;
    
    private Item currentItem;
    private int slotIndex;
    private Depot parentDepot;
    
    // For drag and drop
    private static DepotSlot draggedFrom;
    private static Item draggedItem;
    private static GameObject draggedIconObj;
    
    private void Awake() {
        // Find components if not assigned
        if (itemIcon == null)
            itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
            
        if (quantityText == null && transform.Find("QuantityPanel/QuantityText") != null)
            quantityText = transform.Find("QuantityPanel/QuantityText").GetComponent<TextMeshProUGUI>();
            
        if (quantityPanel == null)
            quantityPanel = transform.Find("QuantityPanel").gameObject;
            
        // Initialize as empty
        ClearSlot();
    }
    
    public void SetIndex(int index) {
        slotIndex = index;
    }
    
    public void SetDepot(Depot depot) {
        parentDepot = depot;
    }
    
    public void SetItem(Item item) {
        currentItem = item;
        
        if (item != null) {
            // Set item icon
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            
            // Show quantity for stackable items
            if (item.isStackable && item.quantity > 1) {
                quantityPanel.SetActive(true);
                quantityText.text = item.quantity.ToString();
            } else {
                quantityPanel.SetActive(false);
            }
        } else {
            ClearSlot();
        }
    }
    
    public void ClearSlot() {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityPanel.SetActive(false);
    }
    
    public Item GetItem() {
        return currentItem;
    }
    
    public int GetIndex() {
        return slotIndex;
    }
    
    // Handle clicks on depot slot
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            // Left click - try to transfer to inventory
            if (currentItem != null && parentDepot != null) {
                parentDepot.TransferToInventory(slotIndex);
            }
        }
    }
    
    // Implement drag and drop interfaces
    public void OnBeginDrag(PointerEventData eventData) {
        if (currentItem != null) {
            draggedFrom = this;
            draggedItem = currentItem;
            
            // Create visual for dragging
            draggedIconObj = new GameObject("DraggedItem");
            draggedIconObj.transform.SetParent(transform.root); // Canvas should be at root
            
            Image dragImage = draggedIconObj.AddComponent<Image>();
            dragImage.sprite = currentItem.icon;
            dragImage.raycastTarget = false;
            
            RectTransform rt = draggedIconObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            
            // Make semi-transparent
            Color c = dragImage.color;
            c.a = 0.7f;
            dragImage.color = c;
        }
    }
    
    public void OnDrag(PointerEventData eventData) {
        if (draggedIconObj != null) {
            draggedIconObj.transform.position = eventData.position;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        if (draggedIconObj != null) {
            Destroy(draggedIconObj);
            
            // If not dropped on a valid target
            if (eventData.pointerCurrentRaycast.gameObject == null || 
                (!eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() && 
                 !eventData.pointerCurrentRaycast.gameObject.GetComponent<DepotSlot>())) {
                // Reset drag references
                draggedFrom = null;
                draggedItem = null;
            }
        }
    }
    
    public void OnDrop(PointerEventData eventData) {
        // Check if item was dragged from inventory
        if (InventorySlot.draggedFrom != null && InventorySlot.draggedItem != null) {
            // Try to add to depot
            if (parentDepot != null) {
                bool added = parentDepot.TransferFromInventory(InventorySlot.draggedItem);
                
                if (added) {
                    // Remove from inventory
                    InventoryManager.Instance.RemoveItem(InventorySlot.draggedFrom.GetIndex(), InventorySlot.draggedItem.quantity);
                    
                    // Clear drag references
                    InventorySlot.draggedFrom = null;
                    InventorySlot.draggedItem = null;
                }
            }
        }
        // Check if item was dragged from another depot slot
        else if (draggedFrom != null && draggedItem != null && draggedFrom != this) {
            // Handle rearranging items within depot
            Item myItem = currentItem;
            
            // Update depot's internal list
            if (parentDepot != null) {
                // Swap items in the depot's Items list
                if (draggedFrom.GetIndex() < parentDepot.Items.Count) {
                    Item temp = null;
                    if (slotIndex < parentDepot.Items.Count) {
                        temp = parentDepot.Items[slotIndex];
                    }
                    
                    // If target slot beyond current Items count, add nulls until we reach it
                    while (slotIndex >= parentDepot.Items.Count) {
                        parentDepot.Items.Add(null);
                    }
                    
                    // Make the swap
                    parentDepot.Items[slotIndex] = parentDepot.Items[draggedFrom.GetIndex()];
                    parentDepot.Items[draggedFrom.GetIndex()] = temp;
                    
                    // Update UI slots
                    SetItem(parentDepot.Items[slotIndex]);
                    draggedFrom.SetItem(temp);
                }
            }
            
            // Clear drag references
            draggedFrom = null;
            draggedItem = null;
        }
    }
}