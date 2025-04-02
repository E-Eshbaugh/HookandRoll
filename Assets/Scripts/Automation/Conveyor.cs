using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Machine
{
    public Direction direction = Direction.Right;
    
    // For visual purposes - animation speed
    public float itemMoveSpeed = 1.0f;
    public float conveyorPlayerSpeed = 1.0f; // Speed at which the player is pushed
    
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
    
    // Override the base SetDirection to handle conveyor-specific positioning
    public override void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        
        // Call base implementation to handle rotation
        base.SetDirection(newDirection);
        
        // Handle conveyor-specific start and end positions
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

    // --- Player Interaction ---

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the "Player" tag
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector2 moveDirection = GetDirectionVector();
                playerMovement.externalVelocity = moveDirection * conveyorPlayerSpeed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Reset external velocity when player leaves the conveyor
                playerMovement.externalVelocity = Vector2.zero;
            }
        }
    }

    // Helper to convert Direction enum to Vector2
    private Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case Direction.Right: return Vector2.right;
            case Direction.Left:  return Vector2.left;
            case Direction.Up:    return Vector2.up;
            case Direction.Down:  return Vector2.down;
            default:              return Vector2.zero;
        }
    }
}