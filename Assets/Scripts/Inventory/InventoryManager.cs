using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    private List<Item> items = new List<Item>();
    private InventorySlot[] slots;
    
    void Start()
    {
        //This currently has a bug, and only finds the hotbar, need to discuss fixes 
        slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        
        /*// Test code
        GameObject prefab = Resources.Load<GameObject>("Carp");
        if (prefab != null) {
            AddItemToInventory(prefab);
        }
        else {
            Debug.LogError("Prefab not found!");
        } */
    }
    
    public void AddItemToInventory(GameObject itemPrefab)
    {
        if (itemPrefab == null) {
            Debug.LogError("Item prefab is null!");
            return;
        }
        
        GameObject itemInstance = Instantiate(itemPrefab);
        Item item = itemInstance.GetComponent<Item>();
        if (item != null)
        {
            AddToInventory(item);
        }
        else
        {
            Debug.LogError("Prefab does not have an Item component");
            Destroy(itemInstance);
        }
    }
    
    public void AddToInventory(Item item)
    {
        // Add to list
        items.Add(item);
        
        // Find empty slot
        InventorySlot emptySlot = FindEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.AddItem(item);
        }
        else
        {
            Debug.LogWarning("No empty slots available!");
            items.Remove(item);
            Destroy(item.gameObject);
        }
    }
    
    private InventorySlot FindEmptySlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                return slot;
            }
        }
        return null;
    }

    public bool hasItem(Item item) {
        foreach (InventorySlot slot in slots) {
            Item check = slot.GetComponentInChildren<Item>();
            if (check != null && check.ItemID == item.ItemID) {
                return true;
            }
        }
        return false;
    }

    public void removeItem(Item item) {
        foreach (InventorySlot slot in slots) {
            Item remove = slot.GetComponentInChildren<Item>();
            if (remove != null && remove.type == item.type) {
                Destroy(remove.gameObject);
                return;
            }
        }
    }
}
