using UnityEngine;
using UnityEngine.Tilemaps;

public class MachinePlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    public GameObject machineController;
    public GameObject conveyorPrefab;
    public GameObject storagePrefab;
    public Grid placementGrid;
    
    private GameObject ghostMachine;
    private Machine.Direction currentDirection = Machine.Direction.Right;
    private bool isPlacing = false;
    
    private Machine lastPlacedMachine = null;
    private GameObject placingMachine;
    
    private void Start()
    {
        // Create a ghost machine for placement preview
        CreateGhostMachine();
    }
    
    private void Update()
    {
        // Toggle placement mode with Space
        if (Input.GetKeyDown(KeyCode.T))
        {
            placingMachine = conveyorPrefab;
            isPlacing = true;
            if (ghostMachine != null)
                SetGhostMachineType(placingMachine);
                ghostMachine.SetActive(isPlacing);
            
        } 
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            placingMachine = storagePrefab;
            isPlacing = true;
            if (ghostMachine != null)
                SetGhostMachineType(placingMachine);
                ghostMachine.SetActive(isPlacing);
            
        } 
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPlacing = false;
            if (ghostMachine != null)
                ghostMachine.SetActive(isPlacing);
        }
        
        // Direction control with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeDirection(Machine.Direction.Up);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeDirection(Machine.Direction.Right);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeDirection(Machine.Direction.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeDirection(Machine.Direction.Left);
        
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

            // Adjust ghost Y position for storage prefab if its pivot is centered differently vertically
            Vector3 ghostPosition = snappedPos;
            if (placingMachine == storagePrefab)
            {
                ghostPosition.y -= placementGrid.cellSize.y / 2;
            }
            
            // Update ghost position
            ghostMachine.transform.position = ghostPosition;
            
            // Place machine on left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                PlaceMachine(snappedPos);
            }
        }
    }
    
    // Create a semi-transparent machine to show placement preview
    private void CreateGhostMachine()
    {
        ghostMachine = Instantiate(conveyorPrefab, Vector3.zero, Quaternion.identity);
        ghostMachine.name = "GhostMachine";
        
        // Make it semi-transparent
        SpriteRenderer[] renderers = ghostMachine.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }
        
        // Disable components that shouldn't run on the ghost
        Machine machine = ghostMachine.GetComponent<Machine>();
        if (machine != null) machine.enabled = false;
        Collider2D ghostCollider = ghostMachine.GetComponent<Collider2D>();
        if (ghostCollider != null) ghostCollider.enabled = false; // Disable collider
        
        // Start with ghost hidden
        ghostMachine.SetActive(false);
    }
    
    private void SetGhostMachineType(GameObject machinePrefab)
    {
        // Destroy existing ghost machine if it exists
        if (ghostMachine != null)
        {
            Destroy(ghostMachine);
        }

        // Create new ghost machine from the provided prefab
        ghostMachine = Instantiate(machinePrefab, Vector3.zero, Quaternion.identity);
        ghostMachine.name = "GhostMachine";
        
        // Make it semi-transparent
        SpriteRenderer[] renderers = ghostMachine.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }
        
        // Disable components that shouldn't run on the ghost
        Machine machine = ghostMachine.GetComponent<Machine>();
        if (machine != null) machine.enabled = false;
        Collider2D ghostCollider = ghostMachine.GetComponent<Collider2D>();
        if (ghostCollider != null) ghostCollider.enabled = false; // Disable collider
        
        // Set the direction to match current direction
        machine.SetDirection(currentDirection);
        
        // Set active state based on current placing mode
        ghostMachine.SetActive(isPlacing);
    }

    // Change the current direction for placement
    private void ChangeDirection(Machine.Direction newDirection)
    {
        currentDirection = newDirection;
        
        if (ghostMachine != null)
        {
            Machine ghostMachineComp = ghostMachine.GetComponent<Machine>();
            if (ghostMachineComp != null)
            {
                ghostMachineComp.SetDirection(currentDirection);
            }
        }
    }
    
    // Place a machine at the specified position
    private void PlaceMachine(Vector3 position)
    {
        // Check if a machine already exists at this position using a smaller collision check
        Vector2 checkSize = new Vector2(0.1f, 0.1f); // Much smaller check size
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, checkSize, 0);
        bool hasMachine = false;
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Machine>() != null)
            {
                // Skip the ghost machine
                if (collider.gameObject.name == "GhostMachine")
                    continue;
                    
                hasMachine = true;
                Debug.Log($"Machine found at position {position}. Collider: {collider.gameObject.name}");
                break;
            }
        }
        
        if (hasMachine)
        {
            return;
        }

        // Adjust Y position for storage prefab if its pivot is centered differently vertically
        Vector3 finalPosition = position;
        if (placingMachine == storagePrefab)
        {
            // Remove the Y half-cell offset applied in Update() because storage pivot seems centered vertically
            // Keep the X offset as its horizontal pivot seems consistent with the conveyor
            finalPosition.y -= placementGrid.cellSize.y / 2;
        }

        // Check if placing on the player
        Collider2D[] playerCheckColliders = Physics2D.OverlapBoxAll(finalPosition, checkSize, 0);
        foreach (Collider2D collider in playerCheckColliders)
        {
            if (collider.CompareTag("Player")) // Assuming player GameObject has "Player" tag
            {
                Debug.Log("Cannot place machine on player.");
                return; // Abort placement
            }
        }
        
        // Create the machine
        GameObject newMachine = Instantiate(placingMachine, finalPosition, Quaternion.identity);

        // Set parent to MachineManager if it exists
        if (MachineManager.Instance != null)
        {
            newMachine.transform.SetParent(MachineManager.Instance.transform);
        }
        // newMachine.name = "Machine";
        
        // Set its direction
        Machine machineComponent = newMachine.GetComponent<Machine>();
        machineComponent.SetDirection(currentDirection);
        // machineComponent.transform.SetParent(machineController);
        
        // Register with machine manager
        if (MachineManager.Instance != null)
        {
            MachineManager.Instance.RegisterMachine(machineComponent);
        }
        
        // If there was a last placed machine, connect them automatically
        if (lastPlacedMachine != null)
        {
            // Check if they are adjacent
            float distance = Vector3.Distance(lastPlacedMachine.transform.position, position);
            if (distance <= placementGrid.cellSize.x * 1.5f)
            {
                // Connect the previous machine to this one
                lastPlacedMachine.ConnectTo(machineComponent);
                Debug.Log("Connected machines");
            }
        }
        
        lastPlacedMachine = machineComponent;
    }
    
    // Spawn a test item on the clicked machine
    public void SpawnTestItem()
    {
        // Raycast to find a machine under the mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            Machine machine = hit.collider.GetComponent<Machine>();
            if (machine != null && MachineManager.Instance != null)
            {
                MachineManager.Instance.SpawnItemOnMachine(machine);
            }
        }
    }
}
