using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    // Direction for item movement
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    // Input and output storages
    public List<Item> inputStorage = new List<Item>();
    public List<Item> outputStorage = new List<Item>();
    
    // Temporary buffer for simulation phases
    protected List<Item> tempOutputStorage = new List<Item>();
    
    // Machine connections
    public List<Machine> inputs = new List<Machine>();
    public List<Machine> outputs = new List<Machine>();
    
    // Storage limits
    public int maxInputCapacity = 5;
    public int maxOutputCapacity = 5;
    
    // Processing state
    protected bool hasProcessedThisTick = false;
    
    // Machine info
    public string machineName = "BaseMachine";
    
    // Production strategy: Forward (just moves items from input to output)
    public enum ProductionStrategy
    {
        Forward
    }
    
    public ProductionStrategy productionStrategy = ProductionStrategy.Forward;
    
    // Visual representation of stored items
    protected List<GameObject> visualItems = new List<GameObject>();
    
    // Initialize connections
    public virtual void Initialize()
    {
        ClearVisualItems();
    }
    
    // Pre-tick phase: Reset processing flags
    public virtual void PreTick()
    {
        hasProcessedThisTick = false;
        tempOutputStorage.Clear();
    }
    
    // Tick phase: Try to process items
    public virtual bool Tick()
    {
        if (hasProcessedThisTick)
            return true;
            
        bool canGrab = TryGrab();
        bool canProduce = TryProduce();
        
        hasProcessedThisTick = canGrab && canProduce;
        return hasProcessedThisTick;
    }
    
    // Post-tick phase: Update storages with temporary buffer
    public virtual void PostTick()
    {
        // Move items from temp storage to output storage
        foreach (Item item in tempOutputStorage)
        {
            outputStorage.Add(item);
        }
        tempOutputStorage.Clear();
        
        // Update visual representation
        UpdateVisualItems();
    }
    
    // Perform a full tick sequence (pre-tick, tick, post-tick)
    public virtual void FullTick()
    {
        PreTick();
        Tick();
        PostTick();
    }
    
    // Try to grab items from inputs
    protected virtual bool TryGrab()
    {
        // Don't grab if input is full
        if (inputStorage.Count >= maxInputCapacity)
            return false;
            
        bool grabbedSomething = false;
        
        // Try to grab from each connected input machine
        foreach (Machine input in inputs)
        {
            // Skip if our input storage is full
            if (inputStorage.Count >= maxInputCapacity)
                break;
                
            // Skip if the input machine has no output
            if (input.outputStorage.Count == 0)
                continue;
                
            // Transfer item from input's output to our input
            Item item = input.outputStorage[0];
            input.outputStorage.RemoveAt(0);
            inputStorage.Add(item);
            
            grabbedSomething = true;
        }
        
        return grabbedSomething || inputs.Count == 0;
    }
    
    // Try to produce items based on strategy
    protected virtual bool TryProduce()
    {
        // Don't produce if output is full
        if (outputStorage.Count >= maxOutputCapacity)
            return false;
            
        // Default Forward strategy: move items from input to temp output
        if (productionStrategy == ProductionStrategy.Forward && inputStorage.Count > 0)
        {
            // Move one item from input to temp output
            Item item = inputStorage[0];
            inputStorage.RemoveAt(0);
            tempOutputStorage.Add(item);
            return true;
        }
        
        return inputStorage.Count == 0; // Success if nothing to process
    }
    
    // Update the visual representation of items
    protected virtual void UpdateVisualItems()
    {
        ClearVisualItems();
        
        // Create visual representations for output items
        for (int i = 0; i < outputStorage.Count; i++)
        {
            GameObject itemObj = new GameObject("Item_" + i);
            itemObj.transform.SetParent(transform);
            
            Item item = itemObj.AddComponent<Item>();
            
            // Position the item on the machine (customize based on your needs)
            float xOffset = 0;
            float yOffset = 0.25f * i;
            item.SetPosition(transform.position + new Vector3(xOffset, yOffset, -0.1f));
            
            visualItems.Add(itemObj);
        }
    }
    
    // Clear all visual items
    protected virtual void ClearVisualItems()
    {
        foreach (GameObject item in visualItems)
        {
            if (item != null)
                Destroy(item);
        }
        visualItems.Clear();
    }
    
    // Connect this machine's output to another machine's input
    public virtual void ConnectTo(Machine other)
    {
        if (!outputs.Contains(other))
            outputs.Add(other);
            
        if (!other.inputs.Contains(this))
            other.inputs.Add(this);
    }

    // Helper method to set the machine's rotation based on direction
    public virtual void SetDirection(Direction newDirection)
    {
        // Update sprite rotation (assuming 0 is right, 90 is up, etc.)
        switch (newDirection)
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
    }
}