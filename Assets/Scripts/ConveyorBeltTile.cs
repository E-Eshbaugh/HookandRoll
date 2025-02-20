using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyorBeltTile : PlaceableBase {
    public ConveyorBeltTile NextTile;
    
    public float MoveDelay = 0.5f;
    private float moveTimer = 0f;
    
    public GameObject CurrentItem { get; set; }

    protected override void Start() {
        base.Start();
        SetupConveyorConnection();
    }

    private void SetupConveyorConnection() {
        Vector3Int adjacentCell = GridPosition + Direction;
        IPlaceable potentialTile = GridManager.Instance.GetPlaceableAt(adjacentCell);
        if (potentialTile is ConveyorBeltTile tile) {
            NextTile = tile;
        }
    }

    // Called to place an item onto this tile.
    public void ReceiveItem(GameObject item) {
        CurrentItem = item;
        item.transform.position = transform.position;
        moveTimer = 0f;
    }

    // Use the internal clock to manage movement.
    protected override void ProcessTick() {
        if (CurrentItem != null) {
            moveTimer += TickInterval;
            if (moveTimer >= MoveDelay && NextTile != null && NextTile.CurrentItem == null) {
                NextTile.ReceiveItem(CurrentItem);
                CurrentItem = null;
                moveTimer = 0f;
            }
        }
    }
}
