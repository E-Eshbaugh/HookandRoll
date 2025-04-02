using System.Collections.Generic;
using UnityEngine;

public class Storage : Machine
{
    public Direction direction = Direction.Right;
    
    // Visual layout configuration
    public float itemSpacing = 0.25f;
    public int itemsPerRow = 4;
    public Vector2 startOffset = new Vector2(-0.375f, 0.375f);
    
    private void Start()
    {
        // Determine start and end positions based on direction
        machineName = "Storage";
        productionStrategy = ProductionStrategy.Forward;
        
        // Storage has larger capacity than regular machines
        maxInputCapacity = 20;
        maxOutputCapacity = 20;
        
        Initialize();
    }
    
    // Override the visual update to show items in a grid layout
    protected override void UpdateVisualItems()
    {
        ClearVisualItems();
        
        // Create visual representations for all stored items
        int totalItems = outputStorage.Count;
        
        for (int i = 0; i < totalItems; i++)
        {
            GameObject itemObj = new GameObject("Item_" + i);
            itemObj.transform.SetParent(transform);
            
            Item item = itemObj.AddComponent<Item>();
            
            // Calculate grid position
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;
            
            // Position items in a grid layout
            float xPos = startOffset.x + (col * itemSpacing);
            float yPos = startOffset.y - (row * itemSpacing);
            
            Vector3 itemPosition = transform.position + new Vector3(xPos, yPos, -0.1f);
            item.SetPosition(itemPosition);
            
            visualItems.Add(itemObj);
        }
    }
    
    // Override the base SetDirection
    public override void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        base.SetDirection(newDirection);
    }
}