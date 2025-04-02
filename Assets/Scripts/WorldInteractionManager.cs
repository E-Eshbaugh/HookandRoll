using UnityEngine;

public class WorldInteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask storageLayerMask; // Assign the Layer(s) your Storage objects are on in the Inspector
    [SerializeField] private KeyCode closeKey = KeyCode.Escape; // Key to close the UI

    private StorageUIManager storageUIManager; // Reference to the manager component on the panel

    void Start()
    {
        // Find the StorageUIManager in the scene.
        // This assumes it's on a child object named "StorageUIPanel" under a Canvas.
        // Adjust the FindObjectOfType or path finding if your structure is different.
        storageUIManager = FindObjectOfType<StorageUIManager>(true); // Include inactive objects

        if (storageUIManager == null)
        {
            Debug.LogError("WorldInteractionManager could not find the StorageUIManager in the scene. Ensure the UI prefab with StorageUIPanel is present.", this);
            enabled = false; // Disable this manager if it can't find the UI manager
        }
        else
        {
             Debug.Log($"WorldInteractionManager found StorageUIManager: {storageUIManager.name} (InstanceID: {storageUIManager.GetInstanceID()})", this);
        }
    }

    void Update()
    {
        // Don't run if UI manager wasn't found
        if (!enabled || storageUIManager == null) return;

        HandleInput();
    }

    private void HandleInput()
    {
        // --- Close UI ---
        // Check if the UI panel itself is active (using the reference)
        if (storageUIManager.gameObject.activeSelf && Input.GetKeyDown(closeKey))
        {
            Debug.Log("Close key pressed."); // DEBUG
            storageUIManager.CloseUI();
            return; // Don't process click if closing
        }

        // --- Open UI on Click ---
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Debug.Log("Mouse button down detected."); // DEBUG
            // Check if clicking on UI element first - ignore world clicks if UI is hit
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                 Debug.Log("Clicked on UI element, ignoring world click."); // DEBUG
                return; // Click was on UI, don't interact with world
            }

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             Debug.Log($"Mouse world position: {mouseWorldPos}"); // DEBUG
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, storageLayerMask);
             Debug.Log($"Raycast performed. LayerMask value: {storageLayerMask.value}"); // DEBUG

            if (hit.collider != null)
            {
                 Debug.Log($"Raycast hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}"); // DEBUG
                Storage clickedStorage = hit.collider.GetComponent<Storage>();
                if (clickedStorage != null)
                {
                     Debug.Log("Found Storage component on hit object."); // DEBUG
                    // Clicked on a storage object - tell the UI Manager to open for this storage
                    storageUIManager.OpenUIForStorage(clickedStorage);
                }
                else
                {
                     Debug.Log("Hit object does not have Storage component."); // DEBUG
                    // Clicked somewhere else in the world while UI was open, close it
                    if (storageUIManager.gameObject.activeSelf)
                    {
                         Debug.Log("Closing UI because click was on non-storage object while UI open."); // DEBUG
                        storageUIManager.CloseUI();
                    }
                }
            }
            else
            {
                 Debug.Log("Raycast did not hit any collider on the specified layer mask."); // DEBUG
                 // Clicked empty space while UI was open, close it
                if (storageUIManager.gameObject.activeSelf)
                {
                     Debug.Log("Closing UI because click was on empty space while UI open."); // DEBUG
                    storageUIManager.CloseUI();
                }
            }
        }
    }
}
