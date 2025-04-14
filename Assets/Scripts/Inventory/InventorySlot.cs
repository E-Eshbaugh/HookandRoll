using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private Item currentItem;
    
    public bool IsEmpty()
    {
        return transform.childCount == 0;
    }
    
    public void OnDrop(PointerEventData eventData) {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        if (item != null)
        {
            // If slot already has an item, swap them
            if (!IsEmpty())
            {
                // Handle swapping logic if needed
                Transform oldParent = item.parentAfterDrag;
                currentItem.transform.SetParent(oldParent);
                currentItem.transform.localPosition = Vector3.zero;
            }
            
            // Add the dropped item
            item.parentAfterDrag = transform;
            currentItem = item;
        }
    }
    
    public void AddItem(Item item)
    {
        // Add the item to this slot
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        currentItem = item;
    }
}