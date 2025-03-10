// using UnityEngine;

// [System.Serializable]
// public class Item 
// {
//     public int id;
//     public string itemName;
//     public int quantity;
//     public Sprite icon; // Added to support inventory UI
//     public string description; // Added for item tooltips
//     public bool isStackable = true; // Most automation items are stackable
    
//     public Item(int id, string itemName, int quantity) 
//     {
//         this.id = id;
//         this.itemName = itemName;
//         this.quantity = quantity;
//     }
    
//     public Item(int id, string itemName, int quantity, Sprite icon, string description = "", bool isStackable = true) 
//     {
//         this.id = id;
//         this.itemName = itemName;
//         this.quantity = quantity;
//         this.icon = icon;
//         this.description = description;
//         this.isStackable = isStackable;
//     }
    
//     // Makes a copy of this item
//     public Item Clone()
//     {
//         return new Item(id, itemName, quantity, icon, description, isStackable);
//     }
    
//     // Used for combining stacks
//     public void AddQuantity(int amount)
//     {
//         quantity += amount;
//     }
    
//     // Used for removing items from stacks
//     public void RemoveQuantity(int amount)
//     {
//         quantity -= amount;
//         if (quantity < 0) quantity = 0;
//     }
// }