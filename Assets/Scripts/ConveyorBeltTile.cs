using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyorBeltTile : PlaceableBase 
{
    public ConveyorBeltTile NextTile;
    public float MoveDelay = 0.5f;
    private float moveTimer = 0f;
    
    [SerializeField] private SpriteRenderer beltRenderer;
    [SerializeField] private Sprite[] directionSprites; // Different sprites for different directions
    
    public GameObject CurrentItem { get; set; }
    
    protected override void Awake() 
    {
        base.Awake();
        
        // Initialize sprite renderer if needed
        if (beltRenderer == null)
        {
            beltRenderer = GetComponent<SpriteRenderer>();
        }
    }
    
    protected override void Start() 
    {
        base.Start();
        SetupConveyorConnection();
        UpdateVisuals();
    }
    
    // Set up connection to next conveyor in chain
    public void SetupConveyorConnection() 
    {
        Vector3Int adjacentCell = GridPosition + Direction;
        IPlaceable potentialTile = GridManager.Instance.GetPlaceableAt(adjacentCell);
        
        if (potentialTile is ConveyorBeltTile tile) 
        {
            NextTile = tile;
        }
    }
    
    // Update the conveyor's visual based on its direction
    private void UpdateVisuals()
    {
        if (beltRenderer != null && directionSprites != null && directionSprites.Length >= 4)
        {
            // Assuming directionSprites array has sprites in order: up, right, down, left
            int dirIndex = 0;
            
            if (Direction == Vector3Int.up) dirIndex = 0;
            else if (Direction == Vector3Int.right) dirIndex = 1;
            else if (Direction == Vector3Int.down) dirIndex = 2;
            else if (Direction == Vector3Int.left) dirIndex = 3;
            
            if (dirIndex < directionSprites.Length)
            {
                beltRenderer.sprite = directionSprites[dirIndex];
            }
        }
    }
    
    // Rotate the conveyor belt (can be called from UI or hotkey)
    public void Rotate()
    {
        // Only allow rotation if no item is on the belt
        if (CurrentItem != null) return;
        
        // Rotate direction 90 degrees clockwise
        if (Direction == Vector3Int.up) Direction = Vector3Int.right;
        else if (Direction == Vector3Int.right) Direction = Vector3Int.down;
        else if (Direction == Vector3Int.down) Direction = Vector3Int.left;
        else if (Direction == Vector3Int.left) Direction = Vector3Int.up;
        
        // Update connections
        SetupConveyorConnection();
        UpdateVisuals();
    }
    
    // Receive an item onto this conveyor
    public void ReceiveItem(GameObject item) 
    {
        if (CurrentItem != null)
        {
            Debug.LogWarning("Trying to place item on occupied conveyor");
            return;
        }
        
        CurrentItem = item;
        
        if (item != null)
        {
            // Position the item on this conveyor
            item.transform.position = transform.position + new Vector3(0, 0.25f, 0); // Slight Y offset
            
            // Set this conveyor as the parent to move with it if needed
            item.transform.SetParent(transform);
            
            moveTimer = 0f;
        }
    }
    
    // Process conveyor logic every tick
    protected override void ProcessTick() 
    {
        if (CurrentItem != null) 
        {
            moveTimer += TickInterval;
            
            if (moveTimer >= MoveDelay) 
            {
                // Check if next tile exists and is free
                if (NextTile != null && NextTile.CurrentItem == null) 
                {
                    // Move item to next conveyor
                    GameObject itemToMove = CurrentItem;
                    CurrentItem = null;
                    itemToMove.transform.SetParent(null);
                    NextTile.ReceiveItem(itemToMove);
                    moveTimer = 0f;
                }
                else if (NextTile == null)
                {
                    // Check if we're pointing at a storage or processing building
                    Vector3Int targetCell = GridPosition + Direction;
                    IPlaceable target = GridManager.Instance.GetPlaceableAt(targetCell);
                    
                    if (target != null)
                    {
                        // Handle different target types
                        if (target is StorageBuilding storage)
                        {
                            ItemComponent itemComp = CurrentItem.GetComponent<ItemComponent>();
                            if (itemComp != null)
                            {
                                bool stored = itemComp.CollectIntoStorage(storage.GetStorageComponent());
                                if (stored)
                                {
                                    CurrentItem = null;
                                    moveTimer = 0f;
                                }
                            }
                        }
                        else if (target is ProcessingBuilding processor)
                        {
                            bool accepted = processor.TryAcceptInput(CurrentItem);
                            if (accepted)
                            {
                                CurrentItem = null;
                                moveTimer = 0f;
                            }
                        }
                    }
                }
            }
        }
    }
    
    // Handle player interaction with the conveyor
    public void OnPlayerInteract()
    {
        // If player clicks on conveyor with item, try to pick it up
        if (CurrentItem != null)
        {
            ItemComponent itemComp = CurrentItem.GetComponent<ItemComponent>();
            if (itemComp != null)
            {
                bool pickedUp = itemComp.PickupItem();
                if (pickedUp)
                {
                    CurrentItem = null;
                }
            }
        }
        else
        {
            // If player clicks on empty conveyor, try to place selected item
            Item selectedItem = HotbarManager.Instance.GetSelectedItem();
            if (selectedItem != null)
            {
                int inventorySlotIndex = HotbarManager.Instance.GetSelectedSlotIndex();
                HotbarManager.Instance.UseSelectedItem(transform.position);
            }
        }
    }
}

// Example classes to support conveyor interactions
public class StorageBuilding : PlaceableBase
{
    [SerializeField] private StorageComponent storageComponent;
    
    public StorageComponent GetStorageComponent()
    {
        return storageComponent;
    }
}

public class ProcessingBuilding : PlaceableBase
{
    [SerializeField] private List<int> acceptedItemIds = new List<int>();
    [SerializeField] private float processingTime = 5f;
    [SerializeField] private int outputItemId = 0;
    [SerializeField] private int outputQuantity = 1;
    
    private float processingTimer = 0f;
    private bool isProcessing = false;
    private GameObject currentInputItem = null;
    
    public bool TryAcceptInput(GameObject inputItem)
    {
        if (isProcessing || inputItem == null) return false;
        
        ItemComponent itemComp = inputItem.GetComponent<ItemComponent>();
        if (itemComp != null && acceptedItemIds.Contains(itemComp.item.id))
        {
            // Accept the item
            currentInputItem = inputItem;
            isProcessing = true;
            processingTimer = 0f;
            
            // Hide the input item while processing
            inputItem.SetActive(false);
            
            return true;
        }
        
        return false;
    }
    
    protected override void ProcessTick()
    {
        if (isProcessing)
        {
            processingTimer += TickInterval;
            
            // Check if processing is complete
            if (processingTimer >= processingTime)
            {
                ProcessingComplete();
            }
        }
    }
    
    private void ProcessingComplete()
    {
        isProcessing = false;
        
        // Destroy the input item
        if (currentInputItem != null)
        {
            Destroy(currentInputItem);
            currentInputItem = null;
        }
        
        // Create the output item
        if (outputItemId > 0)
        {
            // Create output item based on your item database
            Item outputItem = new Item(outputItemId, "Processed Item", outputQuantity);
            GameObject itemObj = InventoryManager.Instance.CreateItemGameObject(outputItem);
            
            // Output the item in the direction the processor is facing
            Vector3Int outputCell = GridPosition + Direction;
            IPlaceable outputTarget = GridManager.Instance.GetPlaceableAt(outputCell);
            
            if (outputTarget is ConveyorBeltTile conveyor && conveyor.CurrentItem == null)
            {
                conveyor.ReceiveItem(itemObj);
            }
            else
            {
                // Place in world at the output position
                Vector3 worldPos = transform.position + new Vector3(Direction.x, Direction.y, 0) * 1.0f;
                itemObj.transform.position = worldPos;
            }
        }
    }
}