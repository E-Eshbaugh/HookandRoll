using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DepotUIManager : MonoBehaviour 
{
    public static DepotUIManager Instance { get; private set; }
    
    [SerializeField] private GameObject depotPanel;
    [SerializeField] private Transform depotSlotsContainer;
    [SerializeField] private GameObject depotSlotPrefab;
    [SerializeField] private TextMeshProUGUI depotTitleText;
    [SerializeField] private Button closeButton;
    
    private Depot currentDepot;
    
    private void Awake() 
    {
        // Singleton pattern
        if (Instance == null) 
        {
            Instance = this;
        } 
        else 
        {
            Destroy(gameObject);
            return;
        }
        
        // Hide depot panel initially
        if (depotPanel != null) 
        {
            depotPanel.SetActive(false);
        }
        
        // Set up close button
        if (closeButton != null) 
        {
            closeButton.onClick.AddListener(CloseDepotPanel);
        }
    }
    
    // Open the depot panel for a specific depot
    public void OpenDepotPanel(Depot depot) 
    {
        if (depot == null || depotPanel == null) return;
        
        currentDepot = depot;
        
        // Set title
        if (depotTitleText != null) 
        {
            depotTitleText.text = $"Depot ({depot.Items.Count}/{depot.Capacity})";
        }
        
        // Show panel
        depotPanel.SetActive(true);
        
        // Create slots if they don't exist
        if (depotSlotsContainer.childCount == 0) 
        {
            for (int i = 0; i < depot.Capacity; i++) 
            {
                GameObject slotObj = Instantiate(depotSlotPrefab, depotSlotsContainer);
                DepotSlot slot = slotObj.GetComponent<DepotSlot>();
                if (slot != null) 
                {
                    slot.SetIndex(i);
                    slot.SetDepot(depot);
                }
            }
        }
        
        // Update slot visuals
        RefreshDepotSlots();
    }
    
    // Close the depot panel
    public void CloseDepotPanel() 
    {
        if (depotPanel != null) 
        {
            depotPanel.SetActive(false);
            currentDepot = null;
        }
    }
    
    // Update slot visuals to match current depot state
    public void RefreshDepotSlots() 
    {
        if (currentDepot == null) return;
        
        for (int i = 0; i < depotSlotsContainer.childCount; i++) 
        {
            DepotSlot slot = depotSlotsContainer.GetChild(i).GetComponent<DepotSlot>();
            if (slot != null) 
            {
                if (i < currentDepot.Items.Count && currentDepot.Items[i] != null) 
                {
                    slot.SetItem(currentDepot.Items[i]);
                } 
                else 
                {
                    slot.ClearSlot();
                }
            }
        }
        
        // Update title
        if (depotTitleText != null) 
        {
            depotTitleText.text = $"Depot ({currentDepot.Items.Count}/{currentDepot.Capacity})";
        }
    }
}