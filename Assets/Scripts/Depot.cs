using UnityEngine;
using System.Collections.Generic;

public class Depot : PlaceableBase {
    public ConveyorBeltTile InputBelt;
    public ConveyorBeltTile OutputBelt;
    
    public int Capacity = 10;
    public List<Item> Items = new List<Item>();
    
    public GameObject ItemPrefab;

    protected override void Start() {
        base.Start();
        SetupConveyorConnections();
    }

    private void SetupConveyorConnections() {
        // Output is in the same direction as the depot's Direction.
        Vector3Int outputOffset = Direction;
        // Input comes from the opposite direction.
        Vector3Int inputOffset = -Direction;

        IPlaceable potentialInput = GridManager.Instance.GetPlaceableAt(GridPosition + inputOffset);
        IPlaceable potentialOutput = GridManager.Instance.GetPlaceableAt(GridPosition + outputOffset);

        if (potentialInput is ConveyorBeltTile inTile) {
            InputBelt = inTile;
        }
        if (potentialOutput is ConveyorBeltTile outTile) {
            OutputBelt = outTile;
        }
    }

    // Automatically push an item onto the output conveyor on each tick.
    protected override void ProcessTick() {
        PushItem();
    }

    public bool AddItem(Item item) {
        if (Items.Count < Capacity) {
            Items.Add(item);
            Debug.Log("Added item: " + item.itemName);
            return true;
        }
        Debug.LogWarning("Depot storage is full.");
        return false;
    }

    public bool RemoveItem(Item item) {
        if (Items.Remove(item)) {
            Debug.Log("Removed item: " + item.itemName);
            return true;
        }
        Debug.LogWarning("Item not found in depot.");
        return false;
    }

    public void PushItem() {
        if (Items.Count > 0 && OutputBelt != null && OutputBelt.CurrentItem == null) {
            GameObject itemObj = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
            ItemComponent itemComp = itemObj.GetComponent<ItemComponent>();
            itemComp.item = Items[0];
            OutputBelt.ReceiveItem(itemObj);
            Items.RemoveAt(0);
        }
    }

    public void PullItem() {
        if (InputBelt != null && InputBelt.CurrentItem != null) {
        GameObject pulledItem = InputBelt.CurrentItem;
        // Extract the ItemComponent that holds the underlying item data.
        ItemComponent itemComp = pulledItem.GetComponent<ItemComponent>();
        if (itemComp != null && itemComp.item != null) {
            if (AddItem(itemComp.item)) {
                Destroy(pulledItem);
                InputBelt.CurrentItem = null;
            }
        }
    }
    }
}
