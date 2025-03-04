using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject quantityPanel;
    [SerializeField] private TextMeshProUGUI slotNumberText;
    
    private Item currentItem;
    private int slotIndex;
    
    // Link to inventory slot - so we know which inventory slot this hotbar item refers to
    private int linkedInventorySlot = -1;

    private void Awake()
    {
        // Find components if not assigned
        if (itemIcon == null)
            itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
            
        if (quantityText == null && transform.Find("QuantityPanel/QuantityText") != null)
            quantityText = transform.Find("QuantityPanel/QuantityText").GetComponent<TextMeshProUGUI>();
            
        if (quantityPanel == null)
            quantityPanel = transform.Find("QuantityPanel").gameObject;
            
        if (slotNumberText == null && transform.Find("SlotNumber") != null)
            slotNumberText = transform.Find("SlotNumber").GetComponent<TextMeshProUGUI>();
        
        // Initialize as empty
        ClearSlot();
    }

    public void SetIndex(int index)
    {
        slotIndex = index;
        
        // Update the slot number display (1-based for UI)
        if (slotNumberText != null)
        {
            if (index < 9)
            {
                slotNumberText.text = (index + 1).ToString();
            }
            else if (index == 9)
            {
                slotNumberText.text = "0";
            }
            else
            {
                slotNumberText.text = "";
            }
        }
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        
        if (item != null)
        {
            // Set item icon
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            
            // Show quantity for stackable items
            if (item.isStackable && item.quantity > 1)
            {
                quantityPanel.SetActive(true);
                quantityText.text = item.quantity.ToString();
            }
            else
            {
                quantityPanel.SetActive(false);
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        linkedInventorySlot = -1;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityPanel.SetActive(false);
    }

    public Item GetItem()
    {
        // Get the latest item from inventory (to ensure quantity is up to date)
        if (linkedInventorySlot >= 0)
        {
            return InventoryManager.Instance.GetItem(linkedInventorySlot);
        }
        return null;
    }

    public int GetIndex()
    {
        return slotIndex;
    }
    
    public void SetLinkedInventorySlot(int inventorySlotIndex)
    {
        linkedInventorySlot = inventorySlotIndex;
    }
    
    public int GetLinkedInventorySlot()
    {
        return linkedInventorySlot;
    }

    // Update quantity display 
    public void UpdateQuantity()
    {
        Item item = GetItem();
        
        if (item != null)
        {
            // Update quantity display
            if (item.isStackable && item.quantity > 1)
            {
                quantityPanel.SetActive(true);
                quantityText.text = item.quantity.ToString();
            }
            else
            {
                quantityPanel.SetActive(false);
            }
            
            // If quantity is zero, clear the slot
            if (item.quantity <= 0)
            {
                ClearSlot();
            }
        }
    }

    // Handle clicks on hotbar slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Select this hotbar slot
            HotbarManager.Instance.SetSelectedSlot(slotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Right-click functionality - perhaps clear the slot
            ClearSlot();
        }
    }

    // Support for dropping inventory items onto hotbar slots
    public void OnDrop(PointerEventData eventData)
    {
        // Check if an item from inventory was dragged here
        if (InventorySlot.draggedFrom != null && InventorySlot.draggedItem != null)
        {
            // Set this hotbar slot to reference the inventory slot
            SetLinkedInventorySlot(InventorySlot.draggedFrom.GetIndex());
            SetItem(InventorySlot.draggedItem);
            
            // Clear static drag references
            InventorySlot.draggedFrom = null;
            InventorySlot.draggedItem = null;
        }
    }
}