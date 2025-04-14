using UnityEngine;

public class AddToInventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get all inventory slots in the scene, use that information to find the first available slot for the item
        InventorySlot[] slots = FindObjectsOfType<InventorySlot>();
        Debug.Log("Found " + slots.Length + " inventory slots in the scene.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
