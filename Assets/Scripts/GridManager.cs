// // GridManager.cs
// using UnityEngine;
// using System.Collections.Generic;

// public class GridManager : MonoBehaviour {
//     public static GridManager Instance;

//     // Maps grid cells to the IPlaceable that occupies them.
//     private Dictionary<Vector3Int, IPlaceable> gridMap = new Dictionary<Vector3Int, IPlaceable>();

//     private void Awake() {
//         Instance = this;
//     }

//     // Registers a placeable if all cells in its footprint are free.
//     public bool RegisterPlaceable(IPlaceable placeable) {
//         foreach (Vector3Int offset in placeable.Footprint) {
//             Vector3Int cell = placeable.GridPosition + offset;
//             if (gridMap.ContainsKey(cell)) {
//                 Debug.LogWarning("Cell already occupied at: " + cell);
//                 return false;
//             }
//         }

//         foreach (Vector3Int offset in placeable.Footprint) {
//             Vector3Int cell = placeable.GridPosition + offset;
//             gridMap[cell] = placeable;
//         }
//         return true;
//     }

//     // Returns the placeable at a given grid cell, if any.
//     public IPlaceable GetPlaceableAt(Vector3Int cellPosition) {
//         gridMap.TryGetValue(cellPosition, out IPlaceable placeable);
//         return placeable;
//     }
// }
