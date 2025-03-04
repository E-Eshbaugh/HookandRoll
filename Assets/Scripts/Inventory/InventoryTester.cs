using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Sprite[] itemIcons;
    public string[] itemNames = { "Flower Pot", "Chest", "Water Bottle" };

    private void Start()
    {
        // Add a longer delay to ensure InventoryManager is fully initialized
        Invoke("AddTestItems", 0.5f);
        
        // Also validate sprite assignments
        for (int i = 0; i < itemIcons.Length; i++)
        {
            if (itemIcons[i] == null)
                Debug.LogError($"Item icon at index {i} is null!");
            else
                Debug.Log($"Item icon at index {i}: {itemIcons[i].name}");
        }
    }
    
    private void AddTestItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager instance is null!");
            return;
        }
        
        // Log inventory size for debugging
        Debug.Log($"Inventory size: {InventoryManager.Instance.GetInventorySize()}");
        
        // Clear inventory first to ensure we start fresh
        InventoryManager.Instance.ClearInventory();
        Debug.Log("Inventory cleared");
        
        // Add test items
        int maxItems = Mathf.Min(itemNames.Length, itemIcons.Length);
        Debug.Log($"Attempting to add {maxItems} items");
        
        for (int i = 0; i < maxItems; i++)
        {
            Item newItem;
            
            // Check if icon is available
            if (i < itemIcons.Length && itemIcons[i] != null)
            {
                // Create item with icon directly in constructor
                newItem = new Item(
                    i + 1, 
                    itemNames[i], 
                    Random.Range(1, 5), 
                    itemIcons[i], 
                    "Test item description"
                );
                Debug.Log($"Adding item: {newItem.itemName} with icon {itemIcons[i].name}");
            }
            else
            {
                Debug.LogWarning($"Missing icon for item {itemNames[i]}");
                // Create basic item without icon
                newItem = new Item(i + 1, itemNames[i], Random.Range(1, 5));
            }
            
            bool added = InventoryManager.Instance.AddItem(newItem);
            Debug.Log($"Item {newItem.itemName} added successfully: {added}");
        }
    }
}