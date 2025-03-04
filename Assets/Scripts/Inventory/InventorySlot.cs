using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject quantityPanel;
    [SerializeField] private Image highlightImage;
    
    private Item currentItem;
    private int slotIndex;
    
    // For drag and drop functionality
    public static InventorySlot draggedFrom;
    public static Item draggedItem;
    private static GameObject draggedIconObj;

    private void Awake()
    {
        // Find components if not assigned
        if (itemIcon == null)
        {
            // First try ItemIcon, then try ImageIcon as fallback
            Transform iconTransform = transform.Find("ItemIcon");
            if (iconTransform == null)
                iconTransform = transform.Find("ImageIcon");
                
            if (iconTransform != null)
                itemIcon = iconTransform.GetComponent<Image>();
            else
                Debug.LogError("Cannot find ItemIcon or ImageIcon in " + gameObject.name);
        }
            
        if (quantityText == null)
        {
            Transform textTransform = transform.Find("QuantityPanel/QuantityText");
            if (textTransform != null)
                quantityText = textTransform.GetComponent<TextMeshProUGUI>();
        }
            
        if (quantityPanel == null)
        {
            Transform panelTransform = transform.Find("QuantityPanel");
            if (panelTransform != null)
                quantityPanel = panelTransform.gameObject;
        }
        
        // Debug current setup
        Debug.Log($"Slot {gameObject.name} setup - Icon: {(itemIcon != null ? "Found" : "Missing")}, " +
                  $"Panel: {(quantityPanel != null ? "Found" : "Missing")}, " +
                  $"Text: {(quantityText != null ? "Found" : "Missing")}");
            
        // Initialize as empty
        ClearSlot();
    }

    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        
        // Make sure components are found (do this check again in case references were lost)
        if (itemIcon == null)
        {
            // First try ItemIcon, then try ImageIcon as fallback
            Transform iconTransform = transform.Find("ItemIcon");
            if (iconTransform == null)
                iconTransform = transform.Find("ImageIcon");
                
            if (iconTransform != null)
                itemIcon = iconTransform.GetComponent<Image>();
        }
        
        if (quantityPanel == null)
        {
            Transform panelTransform = transform.Find("QuantityPanel");
            if (panelTransform != null)
                quantityPanel = panelTransform.gameObject;
        }
        
        if (quantityText == null && quantityPanel != null)
        {
            Transform textTransform = quantityPanel.transform.Find("QuantityText");
            if (textTransform != null)
                quantityText = textTransform.GetComponent<TextMeshProUGUI>();
        }
        
        // Now set the item
        if (item != null)
        {
            // Check if components are assigned
            if (itemIcon == null)
            {
                Debug.LogError("Item icon is still null in slot " + slotIndex + " after retry");
                // Print full child hierarchy for debugging
                Debug.LogError("Child hierarchy: " + GetChildHierarchy(transform));
                return;
            }
            
            // Set item icon if it exists
            if (item.icon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
                
                // Force the sprite to be visible
                itemIcon.color = new Color(1, 1, 1, 1); // Full opacity
                itemIcon.raycastTarget = true;
                
                // Check if the image has a RectTransform and ensure it has proper size
                RectTransform rt = itemIcon.rectTransform;
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }
                
                Debug.Log($"Set sprite {item.icon.name} to slot {slotIndex}");
            }
            else
            {
                Debug.LogWarning("Item " + item.itemName + " has no icon");
                // Use a default sprite or leave disabled
                itemIcon.enabled = false;
            }
            
            // Show quantity for stackable items
            if (item.isStackable && item.quantity > 1 && quantityPanel != null && quantityText != null)
            {
                quantityPanel.SetActive(true);
                quantityText.text = item.quantity.ToString();
            }
            else if (quantityPanel != null)
            {
                quantityPanel.SetActive(false);
            }
        }
        else
        {
            ClearSlot();
        }
    }
    
    // Helper method to get a string representation of the child hierarchy
    private string GetChildHierarchy(Transform parent, string indent = "")
    {
        string result = "";
        foreach (Transform child in parent)
        {
            result += indent + child.name + "\n";
            result += GetChildHierarchy(child, indent + "  ");
        }
        return result;
    }

    public void ClearSlot()
    {
        currentItem = null;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        
        if (quantityPanel != null)
        {
            quantityPanel.SetActive(false);
        }
    }

    public Item GetItem()
    {
        return currentItem;
    }

    public int GetIndex()
    {
        return slotIndex;
    }

    // Handle clicks on inventory slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Left click - select for hotbar or use
            if (currentItem != null)
            {
                HotbarManager.Instance.SetSelectedSlot(slotIndex);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Right click - for placing in the world or showing details
            if (currentItem != null)
            {
                // Show details or handle right-click functionality
                Debug.Log($"Item: {currentItem.itemName} - {currentItem.description}");
            }
        }
    }

    // Drag and drop implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            draggedFrom = this;
            draggedItem = currentItem;
            
            // Create a visual representation for dragging
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

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIconObj != null)
        {
            draggedIconObj.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIconObj != null)
        {
            Destroy(draggedIconObj);
            
            // If not dropped on a valid target (like a hotbar slot)
            if (eventData.pointerCurrentRaycast.gameObject == null || 
                (!eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() && 
                 !eventData.pointerCurrentRaycast.gameObject.GetComponent<HotbarSlot>()))
            {
                // For automation game - handle dropping items into the world
                // Check if we're over a conveyor belt, storage, etc.
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0; // Set to 0 for 2D
                
                Grid grid = FindFirstObjectByType<Grid>();
                if (grid != null)
                {
                    Vector3Int cellPos = grid.WorldToCell(worldPos);
                    IPlaceable placeable = GridManager.Instance.GetPlaceableAt(cellPos);
                    
                    if (placeable is ConveyorBeltTile conveyor && conveyor.CurrentItem == null)
                    {
                        // Place item on conveyor
                        GameObject itemObj = InventoryManager.Instance.CreateItemGameObject(draggedItem);
                        conveyor.ReceiveItem(itemObj);
                        
                        // Remove one from inventory
                        InventoryManager.Instance.RemoveItem(draggedFrom.slotIndex, 1);
                    }
                    // Add more conditions for other placeables
                }
            }
            
            draggedFrom = null;
            draggedItem = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFrom != null && draggedItem != null)
        {
            // Handle item swapping between inventory slots
            Item myItem = currentItem;
            SetItem(draggedItem);
            draggedFrom.SetItem(myItem);
            
            draggedFrom = null;
            draggedItem = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.gameObject.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.gameObject.SetActive(false);
    }
}