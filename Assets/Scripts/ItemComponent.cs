// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class ItemComponent : MonoBehaviour 
// {
//     public Item item;
//     private SpriteRenderer spriteRenderer;
    
//     private void Awake()
//     {
//         // Initialize sprite renderer if not already attached
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         if (spriteRenderer == null)
//         {
//             spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
//         }
//     }
    
//     private void Start()
//     {
//         UpdateVisuals();
//     }
    
//     // Update the visual representation of this item
//     public void UpdateVisuals()
//     {
//         if (item != null && spriteRenderer != null)
//         {
//             if (item.icon != null)
//             {
//                 spriteRenderer.sprite = item.icon;
//             }
            
//             // Scale the sprite to a reasonable size for display on conveyor belts
//             transform.localScale = new Vector3(0.5f, 0.5f, 1f);
//         }
//     }
    
//     // Pick up this item into the player's inventory
//     public bool PickupItem()
//     {
//         if (item != null)
//         {
//             bool added = InventoryManager.Instance.AddItem(item);
//             if (added)
//             {
//                 // Destroy the physical representation since it's now in inventory
//                 Destroy(gameObject);
//                 return true;
//             }
//         }
//         return false;
//     }
    
//     // Collect item into a specific container (e.g., storage building)
//     public bool CollectIntoStorage(StorageComponent storage)
//     {
//         if (item != null && storage != null)
//         {
//             bool added = storage.AddItem(item);
//             if (added)
//             {
//                 Destroy(gameObject);
//                 return true;
//             }
//         }
//         return false;
//     }
// }

// // Example of a storage component that could be attached to storage buildings
// public class StorageComponent : MonoBehaviour
// {
//     [SerializeField] private int maxCapacity = 50;
//     [SerializeField] private List<Item> storedItems = new List<Item>();
    
//     public bool AddItem(Item itemToAdd)
//     {
//         if (itemToAdd == null) return false;
        
//         // Check storage capacity
//         int currentQuantity = 0;
//         foreach (Item item in storedItems)
//         {
//             currentQuantity += item.quantity;
//         }
        
//         if (currentQuantity + itemToAdd.quantity > maxCapacity)
//         {
//             Debug.Log("Storage is full!");
//             return false;
//         }
        
//         // Try to stack with existing items
//         if (itemToAdd.isStackable)
//         {
//             foreach (Item item in storedItems)
//             {
//                 if (item.id == itemToAdd.id)
//                 {
//                     item.AddQuantity(itemToAdd.quantity);
//                     return true;
//                 }
//             }
//         }
        
//         // Add as new item
//         storedItems.Add(itemToAdd.Clone());
//         return true;
//     }
    
//     // Method to retrieve an item from storage
//     public Item RetrieveItem(int itemId, int amount)
//     {
//         foreach (Item item in storedItems)
//         {
//             if (item.id == itemId)
//             {
//                 int amountToTake = Mathf.Min(amount, item.quantity);
//                 item.RemoveQuantity(amountToTake);
                
//                 if (item.quantity <= 0)
//                 {
//                     storedItems.Remove(item);
//                 }
                
//                 // Return a new item with the taken quantity
//                 return new Item(itemId, item.itemName, amountToTake, item.icon, item.description, item.isStackable);
//             }
//         }
        
//         return null;
//     }
// }