using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Machine
{
    // Direction for item movement
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public Direction direction = Direction.Right;
    
    // For visual purposes - animation speed
    public float itemMoveSpeed = 1.0f;
    
    // Transform positions for item visualization
    private Vector3 startPosition;
    private Vector3 endPosition;
    
    private void Start()
    {
        // Determine start and end positions based on direction
        float offset = 0.4f; // Half the conveyor length
        
        switch (direction)
        {
            case Direction.Right:
                startPosition = transform.position + new Vector3(-offset, 0, 0);
                endPosition = transform.position + new Vector3(offset, 0, 0);
                break;
            case Direction.Left:
                startPosition = transform.position + new Vector3(offset, 0, 0);
                endPosition = transform.position + new Vector3(-offset, 0, 0);
                break;
            case Direction.Up:
                startPosition = transform.position + new Vector3(0, -offset, 0);
                endPosition = transform.position + new Vector3(0, offset, 0);
                break;
            case Direction.Down:
                startPosition = transform.position + new Vector3(0, offset, 0);
                endPosition = transform.position + new Vector3(0, -offset, 0);
                break;
        }
        
        machineName = "Conveyor";
        productionStrategy = ProductionStrategy.Forward;
        
        Initialize();
    }
    
    // Override the visual update to show items moving along the conveyor
    protected override void UpdateVisualItems()
    {
        ClearVisualItems();
        
        // Create visual representations for output items
        for (int i = 0; i < outputStorage.Count; i++)
        {
            GameObject itemObj = new GameObject("Item_" + i);
            itemObj.transform.SetParent(transform);
            
            Item item = itemObj.AddComponent<Item>();
            
            // Position items along the conveyor based on their index
            float progress = (float)i / (maxOutputCapacity - 1);
            if (float.IsNaN(progress)) progress = 0; // Handle division by zero
            
            // Lerp between start and end positions
            Vector3 itemPosition = Vector3.Lerp(endPosition, startPosition, progress);
            item.SetPosition(itemPosition);
            
            visualItems.Add(itemObj);
        }
    }
    
    // Helper method to set the conveyor's rotation based on direction
    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        
        // Update sprite rotation (assuming 0 is right, 90 is up, etc.)
        switch (direction)
        {
            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
        }
        
        // Recalculate start and end positions
        float offset = 0.4f;
        
        switch (direction)
        {
            case Direction.Right:
                startPosition = transform.position + new Vector3(-offset, 0, 0);
                endPosition = transform.position + new Vector3(offset, 0, 0);
                break;
            case Direction.Left:
                startPosition = transform.position + new Vector3(offset, 0, 0);
                endPosition = transform.position + new Vector3(-offset, 0, 0);
                break;
            case Direction.Up:
                startPosition = transform.position + new Vector3(0, -offset, 0);
                endPosition = transform.position + new Vector3(0, offset, 0);
                break;
            case Direction.Down:
                startPosition = transform.position + new Vector3(0, offset, 0);
                endPosition = transform.position + new Vector3(0, -offset, 0);
                break;
        }
    }
}