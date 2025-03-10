// using UnityEngine;
// using UnityEngine.EventSystems;

// public class PlayerInteraction : MonoBehaviour
// {
//     public Camera mainCamera;
//     public float interactionDistance = 5f;
//     public LayerMask interactableLayers;

//     public GameObject objectToToggle; // Assign this in the inspector

//     [SerializeField] private GameObject placementPreviewPrefab;
//     private GameObject currentPlacementPreview;

//     private bool isPlacingMode = false;
//     private int selectedPlaceableItemId = -1;

//     private void Awake()
//     {
//         if (mainCamera == null)
//         {
//             mainCamera = Camera.main;
//         }
//     }

//     private void Update()
//     {
//         // Skip interaction if mouse is over UI
//         if (EventSystem.current.IsPointerOverGameObject())
//         {
//             return;
//         }

//         // Get world position from mouse
//         Vector3 mousePosition = Input.mousePosition;
//         mousePosition.z = -mainCamera.transform.position.z;
//         Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

//         // For grid-based positioning
//         Grid grid = FindFirstObjectByType<Grid>();
//         if (grid != null)
//         {
//             Vector3Int cellPosition = grid.WorldToCell(worldPosition);
//             worldPosition = grid.GetCellCenterWorld(cellPosition);
//         }

//         // When in placement mode, show a preview
//         Item selectedItem = HotbarManager.Instance.GetSelectedItem();
//         if (selectedItem != null)
//         {
//             // Check if this is a placeable building item
//             if (IsPlaceableItem(selectedItem.id))
//             {
//                 isPlacingMode = true;
//                 selectedPlaceableItemId = selectedItem.id;
//                 UpdatePlacementPreview(worldPosition);
//             }
//             else
//             {
//                 isPlacingMode = false;
//                 ClearPlacementPreview();
//             }
//         }
//         else
//         {
//             isPlacingMode = false;
//             ClearPlacementPreview();
//         }

//         // Handle input
//         if (Input.GetMouseButtonDown(0)) // Left click
//         {
//             if (isPlacingMode)
//             {
//                 PlaceSelectedObject(worldPosition);
//             }
//             else
//             {
//                 // Handle interaction with existing objects
//                 Interact(worldPosition);
//             }
//         }
//         else if (Input.GetMouseButtonDown(1)) // Right click
//         {
//             // Cancel placement or rotate placement preview
//             if (isPlacingMode && currentPlacementPreview != null)
//             {
//                 RotatePlacementPreview();
//             }
//         }

//         // Hotkey for picking up items
//         if (Input.GetKeyDown(KeyCode.F))
//         {
//             PickUpNearbyItem(worldPosition);
//         }

//         // Toggle visibility
//         if (Input.GetKeyDown(KeyCode.V))
//         {
//             if (objectToToggle != null)
//             {
//                 objectToToggle.SetActive(!objectToToggle.activeSelf);
//             }
//             else
//             {
//                 Debug.LogWarning("objectToToggle is not assigned in the inspector!");
//             }
//         }
//     }

//     private bool IsPlaceableItem(int itemId)
//     {
//         // Define which item IDs correspond to placeable structures
//         return (itemId == 1 || itemId == 2 || itemId == 3); // Example: 1=conveyor, 2=storage, 3=machine
//     }

//     private void UpdatePlacementPreview(Vector3 position)
//     {
//         // Create preview if it doesn't exist
//         if (currentPlacementPreview == null)
//         {
//             // Load the appropriate prefab based on the selected item type
//             GameObject prefab = null;

//             switch (selectedPlaceableItemId)
//             {
//                 case 1: // Conveyor
//                     prefab = Resources.Load<GameObject>("Prefabs/ConveyorBelt");
//                     break;
//                 case 2: // Storage
//                     prefab = Resources.Load<GameObject>("Prefabs/Storage");
//                     break;
//             }

//             if (prefab != null)
//             {
//                 currentPlacementPreview = Instantiate(prefab, position, Quaternion.identity);

//                 // Set transparency to indicate it's a preview
//                 SpriteRenderer[] renderers = currentPlacementPreview.GetComponentsInChildren<SpriteRenderer>();
//                 foreach (SpriteRenderer renderer in renderers)
//                 {
//                     Color color = renderer.color;
//                     color.a = 0.5f;
//                     renderer.color = color;
//                 }

//                 // Disable any scripts that might interfere
//                 MonoBehaviour[] scripts = currentPlacementPreview.GetComponentsInChildren<MonoBehaviour>();
//                 foreach (MonoBehaviour script in scripts)
//                 {
//                     script.enabled = false;
//                 }
//         Grid grid = FindFirstObjectByType<Grid>();
//         if (grid != null)
//         {
//             Vector3Int cellPosition = grid.WorldToCell(position);
//             bool canPlace = GridManager.Instance.GetPlaceableAt(cellPosition) == null;
            
//             // Visual feedback based on validity
//             SpriteRenderer[] renderers = currentPlacementPreview.GetComponentsInChildren<SpriteRenderer>();
//             foreach (SpriteRenderer renderer in renderers)
//             {
//                 renderer.color = canPlace ? new Color(0.5f, 1f, 0.5f, 0.5f) : new Color(1f, 0.5f, 0.5f, 0.5f);
//             }
//         }
//     }
    
//     private void ClearPlacementPreview()
//     {
//         if (currentPlacementPreview != null)
//         {
//             Destroy(currentPlacementPreview);
//             currentPlacementPreview = null;
//         }
//     }
    
//     private void RotatePlacementPreview()
//     {
//         if (currentPlacementPreview != null)
//         {
//             // Rotate the preview object
//             currentPlacementPreview.transform.Rotate(0, 0, 90);
            
//             // Update the direction for the actual placement
//             if (currentPlacementPreview.TryGetComponent<PlaceableBase>(out var placeable))
//             {
//                 // Rotate direction vector 90 degrees clockwise
//                 Vector3Int currentDir = placeable.Direction;
//                 if (currentDir == Vector3Int.up) placeable.Direction = Vector3Int.right;
//                 else if (currentDir == Vector3Int.right) placeable.Direction = Vector3Int.down;
//                 else if (currentDir == Vector3Int.down) placeable.Direction = Vector3Int.left;
//                 else if (currentDir == Vector3Int.left) placeable.Direction = Vector3Int.up;
//             }
//         }
//     }
    
//     private void PlaceSelectedObject(Vector3 position)
//     {
//         Grid grid = FindFirstObjectByType<Grid>();
//         if (grid != null)
//         {
//             Vector3Int cellPosition = grid.WorldToCell(position);
            
//             // Check if cell is empty
//             if (GridManager.Instance.GetPlaceableAt(cellPosition) == null)
//             {
//                 // Get item from hotbar
//                 HotbarManager.Instance.UseSelectedItem(position);
                
//                 // After successful placement, auto-setup connections for conveyors
//                 IPlaceable placeable = GridManager.Instance.GetPlaceableAt(cellPosition);
//                 if (placeable is ConveyorBeltTile conveyor)
//                 {
//                     // Copy rotation from preview
//                     if (currentPlacementPreview != null && currentPlacementPreview.TryGetComponent<PlaceableBase>(out var previewPlaceable))
//                     {
//                         conveyor.Direction = previewPlaceable.Direction;
//                     }
                    
//                     // Setup connections with nearby conveyors
//                     conveyor.SetupConveyorConnection();
//                 }
//             }
//         }
//     }
    
//     private void Interact(Vector3 position)
//     {
//         // Cast a ray to check for interactable objects
//         RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0.1f, interactableLayers);
        
//         if (hit.collider != null)
//         {
//             // Check for different interactable types
//             if (hit.collider.TryGetComponent<ItemComponent>(out var item))
//             {
//                 // Pickup loose item in the world
//                 item.PickupItem();
//             }
//             else if (hit.collider.TryGetComponent<ConveyorBeltTile>(out var conveyor))
//             {
//                 // Interact with conveyor
//                 conveyor.OnPlayerInteract();
//             }
//             else if (hit.collider.TryGetComponent<StorageBuilding>(out var storage))
//             {
//                 // Open storage UI
//                 // Implementation depends on your UI system
//                 Debug.Log("Opening storage UI");
//             }
//         }
//     }
    
//     private void PickUpNearbyItem(Vector3 position)
//     {
//         // Find items in a small radius around the click point
//         Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        
//         foreach (Collider2D collider in colliders)
//         {
//             if (collider.TryGetComponent<ItemComponent>(out var item))
//             {
//                 bool pickedUp = item.PickupItem();
//                 if (pickedUp) break; // Only pick up one item at a time
//             }
//         }
//     }
// }