// using UnityEngine;
// using UnityEngine.EventSystems;

// public class PlayerDepotInteraction : MonoBehaviour 
// {
//     [SerializeField] private Camera mainCamera;
//     [SerializeField] private float interactionDistance = 3f;
//     [SerializeField] private LayerMask interactableLayers;
    
//     private void Awake() 
//     {
//         if (mainCamera == null) 
//         {
//             mainCamera = Camera.main;
//         }
//     }
    
//     private void Update() 
//     {
//         // Check for mouse click
//         if (Input.GetMouseButtonDown(0)) 
//         {
//             // Skip if clicking on UI elements
//             if (EventSystem.current.IsPointerOverGameObject()) 
//             {
//                 return;
//             }
            
//             // Raycast to check for interactable objects
//             Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//             RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, interactionDistance, interactableLayers);
            
//             if (hit.collider != null) 
//             {
//                 // Check if we hit a depot
//                 Depot depot = hit.collider.GetComponent<Depot>();
//                 if (depot != null) 
//                 {
//                     // Open the depot UI
//                     DepotUIManager.Instance.OpenDepotPanel(depot);
//                 }
//             }
//         }
        
//         // Close depot UI with Escape key
//         if (Input.GetKeyDown(KeyCode.Escape)) 
//         {
//             DepotUIManager.Instance.CloseDepotPanel();
//         }
//     }
// }