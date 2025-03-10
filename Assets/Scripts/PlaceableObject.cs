// // PlaceableObject.cs
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public abstract class PlaceableBase : MonoBehaviour, IPlaceable {
//     public Vector3Int GridPosition { get; set; }
//     public virtual List<Vector3Int> Footprint { get; } = new List<Vector3Int> { Vector3Int.zero };
//     public Vector3Int Direction { get; set; } = Vector3Int.up;
    
//     public float TickInterval = 0.5f;

//     protected virtual void Awake() {
//         Grid grid = FindFirstObjectByType<Grid>();
//         if (grid != null) {
//             GridPosition = grid.WorldToCell(transform.position);
//             bool registered = GridManager.Instance.RegisterPlaceable(this);
//             if (!registered) {
//                 Debug.LogError("Failed to register placeable at: " + GridPosition);
//             }
//         }
//     }

//     protected virtual void Start() {
//         StartCoroutine(InternalClock());
//     }

//     private IEnumerator InternalClock() {
//         while (true) {
//             yield return new WaitForSeconds(TickInterval);
//             ProcessTick();
//         }
//     }

//     // Override this in derived classes to perform per‑tick logic.
//     protected virtual void ProcessTick() {
//         // void
//     }
// }
