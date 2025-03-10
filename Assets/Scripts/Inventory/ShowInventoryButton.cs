using UnityEngine;

public class ToggleVisibilityOnKeyPress : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle; // Object whose visibility will be toggled
    [SerializeField] private KeyCode toggleKey = KeyCode.Space; // Default to spacebar, can be changed in Inspector
    
    private void Start()
    {
        // Ensure we have a reference to the object
        if (objectToToggle == null)
        {
            Debug.LogError("No object assigned to toggle visibility!");
        }
    }
    
    private void Update()
    {
        // Check if the specified key was pressed this frame
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleVisibility();
        }
    }
    
    private void ToggleVisibility()
    {
        if (objectToToggle != null)
        {
            // Toggle the active state which controls visibility
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}