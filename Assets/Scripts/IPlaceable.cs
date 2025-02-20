using UnityEngine;
using System.Collections.Generic;

public interface IPlaceable {
    Vector3Int GridPosition { get; set; }
    Vector3Int Direction { get; set; } // 1 north, 2 east, 3 south, 4 west
    // For future support of multi tile objects
    List<Vector3Int> Footprint { get; }
}