using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // inventory is set up into two separate parts, so we need both
    public Transform inventoryToolbar;
    public Transform inventoryMain;
    
    // function to check if we already have an item of a certain type
    public bool HaveItemOfType(Item.ItemType type) {
        foreach (Transform slot in inventoryToolbar) {
            Item item = slot.GetComponentInChildren<Item>();
            if (item != null && item.type == type) {
                return true;
            }
        }
        foreach (Transform slot in inventoryMain) {
             Item item = slot.GetComponentInChildren<Item>();
            if (item != null && item.type == type) {
                return true;
            }
        }
        return false;
    }

    public bool addItem(Item itemPrefab) {
        foreach (Transform slot in inventoryToolbar) {
            if (slot.childCount == 0) {
                Item newItem = Instantiate(itemPrefab, slot);
                newItem.transform.localPosition = Vector2.zero;

                Item itemScript = newItem.GetComponent<Item>();
                if (itemScript != null) {
                    itemScript.parentAfterDrag = slot;
                }
                return true;
            }
        }
        foreach (Transform slot in inventoryMain) {
            if (slot.childCount == 0) {
                Item newItem = Instantiate(itemPrefab, slot);
                newItem.transform.localPosition = Vector2.zero;

                Item itemScript = newItem.GetComponent<Item>();
                if (itemScript != null) {
                    itemScript.parentAfterDrag = slot;
                }
                return true;
            }
        }
        Debug.LogWarning("No open inventory slots");
        return false;
    }
}
