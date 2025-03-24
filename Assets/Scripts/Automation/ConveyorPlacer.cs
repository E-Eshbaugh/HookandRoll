using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorPlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    public GameObject conveyorPrefab;
    public Grid placementGrid;
    
    private GameObject ghostConveyor;
    private Conveyor.Direction currentDirection = Conveyor.Direction.Right;
    private bool isPlacing = false;
    
    private Conveyor lastPlacedConveyor = null;
    
    private void Start()
    {
        // Create a ghost conveyor for placement preview
        CreateGhostConveyor();
    }
    
    private void Update()
    {
        // Toggle placement mode with Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPlacing = !isPlacing;
            if (ghostConveyor != null)
                ghostConveyor.SetActive(isPlacing);
        }
        
        // Direction control with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeDirection(Conveyor.Direction.Up);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeDirection(Conveyor.Direction.Right);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeDirection(Conveyor.Direction.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeDirection(Conveyor.Direction.Left);
        
        if (isPlacing)
        {
            // Get mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            // Snap to grid
            Vector3Int cellPos = placementGrid.WorldToCell(mouseWorldPos);
            Vector3 snappedPos = placementGrid.CellToWorld(cellPos);
            snappedPos.x += placementGrid.cellSize.x / 2;
            snappedPos.y += placementGrid.cellSize.y / 2;
            snappedPos.z = 0;
            
            // Update ghost position
            ghostConveyor.transform.position = snappedPos;
            
            // Place conveyor on left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                PlaceConveyor(snappedPos);
            }
        }
    }
    
    // Create a semi-transparent conveyor to show placement preview
    private void CreateGhostConveyor()
    {
        ghostConveyor = Instantiate(conveyorPrefab, Vector3.zero, Quaternion.identity);
        ghostConveyor.name = "GhostConveyor";
        
        // Make it semi-transparent
        SpriteRenderer[] renderers = ghostConveyor.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }
        
        // Disable components that shouldn't run on the ghost
        Conveyor conveyor = ghostConveyor.GetComponent<Conveyor>();
        conveyor.enabled = false;
        
        // Start with ghost hidden
        ghostConveyor.SetActive(false);
    }
    
    // Change the current direction for placement
    private void ChangeDirection(Conveyor.Direction newDirection)
    {
        currentDirection = newDirection;
        
        if (ghostConveyor != null)
        {
            Conveyor ghostConvComp = ghostConveyor.GetComponent<Conveyor>();
            if (ghostConvComp != null)
            {
                ghostConvComp.SetDirection(currentDirection);
            }
        }
    }
    
    // Place a conveyor at the specified position
    private void PlaceConveyor(Vector3 position)
    {
        // Check if a conveyor already exists at this position
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, placementGrid.cellSize * 0.8f, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Conveyor>() != null)
            {
                Debug.Log("Conveyor already exists at this position");
                return;
            }
        }
        
        // Create the conveyor
        GameObject newConveyor = Instantiate(conveyorPrefab, position, Quaternion.identity);
        newConveyor.name = "Conveyor";
        
        // Set its direction
        Conveyor conveyorComponent = newConveyor.GetComponent<Conveyor>();
        conveyorComponent.SetDirection(currentDirection);
        
        // Register with machine manager
        if (MachineManager.Instance != null)
        {
            MachineManager.Instance.RegisterMachine(conveyorComponent);
        }
        
        // If there was a last placed conveyor, connect them automatically
        if (lastPlacedConveyor != null)
        {
            // Check if they are adjacent
            float distance = Vector3.Distance(lastPlacedConveyor.transform.position, position);
            if (distance <= placementGrid.cellSize.x * 1.5f)
            {
                // Connect the previous conveyor to this one
                lastPlacedConveyor.ConnectTo(conveyorComponent);
                Debug.Log("Connected conveyors");
            }
        }
        
        lastPlacedConveyor = conveyorComponent;
    }
    
    // Spawn a test item on the clicked conveyor
    public void SpawnTestItem()
    {
        // Raycast to find a conveyor under the mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            Conveyor conveyor = hit.collider.GetComponent<Conveyor>();
            if (conveyor != null && MachineManager.Instance != null)
            {
                MachineManager.Instance.SpawnItemOnMachine(conveyor);
            }
        }
    }
}