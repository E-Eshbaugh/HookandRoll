using UnityEngine;

public class ToggleStorageUIOnClick : MonoBehaviour
{
    // Removed: [SerializeField] private GameObject storageUIPanelPrefab;
    [SerializeField] private Canvas mainCanvas; // Assign the main UI Canvas (or it will try to find one)
    [SerializeField] private KeyCode closeKey = KeyCode.Escape; // Key to close the UI

    private GameObject storageUIPanelGameObject; // Reference to the panel within the main UI
    private StorageUIManager storageUIManager; // Reference to the manager component on the panel
    private Storage myStorageComponent;

    private void Awake()
    {
        // Get the Storage component on this same GameObject
        myStorageComponent = GetComponent<Storage>();
        if (myStorageComponent == null)
        {
            Debug.LogError("ToggleStorageUIOnClick requires a Storage component on the same GameObject.", this);
            enabled = false; // Disable script if Storage component is missing
        }

        // Find the main canvas if not assigned
        if (mainCanvas == null)
        {
            mainCanvas = FindFirstObjectByType<Canvas>();
            if (mainCanvas == null)
            {
                 Debug.LogError("Could not find a Canvas in the scene. Please assign it in the Inspector.", this);
                 enabled = false;
                 return; // Stop if canvas not found
            }
        }

       // Find the StorageUIPanel within the canvas hierarchy
       // Assuming StorageUIPanel is a direct child of the Canvas GameObject found/assigned.
       // Adjust the path if it's nested deeper, e.g., mainCanvas.transform.Find("SomeGroup/StorageUIPanel");
       Transform panelTransform = mainCanvas.transform.Find("StorageUIPanel");
       if (panelTransform == null)
       {
           Debug.LogError($"Could not find child GameObject named 'StorageUIPanel' under the Canvas '{mainCanvas.name}'. Please check the UI prefab structure.", mainCanvas);
           enabled = false;
           return;
       }
       storageUIPanelGameObject = panelTransform.gameObject;

       storageUIManager = storageUIPanelGameObject.GetComponent<StorageUIManager>();
       if (storageUIManager == null)
       {
           Debug.LogError($"The found 'StorageUIPanel' GameObject is missing the required StorageUIManager component.", storageUIPanelGameObject);
           enabled = false;
           return;
       }

       // Ensure the panel starts inactive (it should be inactive in the prefab already, but good practice)
       storageUIPanelGameObject.SetActive(false);
    }

    private void Update()
    {
        // Check if the UI panel exists, is active, and the close key is pressed
        if (storageUIPanelGameObject != null && storageUIPanelGameObject.activeSelf && Input.GetKeyDown(closeKey))
        {
            ToggleUI(false); // Force close
        }
    }

    // This method is called when the GameObject this script is attached to is clicked
    // Requires a Collider2D component on the GameObject and an EventSystem in the scene
    private void OnMouseDown()
    {
        // Check if script is enabled, has storage component, and the UI panel was found
        if (enabled && myStorageComponent != null && storageUIPanelGameObject != null && storageUIManager != null)
        {
            // Toggle based on the current active state of the panel
            ToggleUI(!storageUIPanelGameObject.activeSelf);
        }
        else
        {
             Debug.LogWarning("Cannot toggle UI. Check component references and ensure script is enabled.", this);
        }
    }

    private void ToggleUI(bool show)
    {
        if (show)
        {
            // We now use the panel found in Awake
            if (storageUIPanelGameObject == null || storageUIManager == null)
            {
                 Debug.LogError("Storage UI Panel or Manager not found/initialized correctly. Cannot open UI.", this);
                 return;
            }

            // Set the target storage for the UI manager and activate the panel
            storageUIManager.SetTargetStorage(myStorageComponent); // Tell the manager which storage to display
            storageUIPanelGameObject.SetActive(true); // Activate the panel
            // The UIManager's OnEnable will call UpdateUI()
        }
        else
        {
            // Deactivate the panel if it exists
            // Deactivate the panel if it exists and is assigned
            if (storageUIPanelGameObject != null)
            {
                storageUIPanelGameObject.SetActive(false);
            }
        }

        // Optional: Log the state change
        // Debug.Log($"Storage UI for {gameObject.name} {(show ? "opened" : "closed")}");
    }

    // Removed OnDestroy cleanup as we no longer instantiate the panel
}
