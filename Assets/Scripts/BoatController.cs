using System;
using System.Numerics;
using UnityEditor.Callbacks;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Animator boatAnimator;
    public GameObject Compass; // Assign the parent Compass GameObject in the Inspector
    public GameObject sailIndicator;
    public Boolean inBoat = false;
    public Boolean anchored = false;
    // [SerializeField] private GameObject sailIndicator;
    private bool isRowingMode = false; // Flag for the new movement mode
    [Header("-- Player Interaction Setup --")]
    public SpriteRenderer playerMainRenderer;
    public MonoBehaviour playerMovementScript;
    public Transform player;
    public UnityEngine.Vector3 playerOffset = new UnityEngine.Vector3(0, 0, 0);
    [Header("-- Sailing Physics -- ")]
    public float sailAngle;
    public float windSpeed;
    public float scaledWindSpeed;
    public UnityEngine.Vector2 windDirection;
    public float sailsUpScaler = 0.0f;
    public float topSpeed = 5f;
    public float waterResistanceFactor = 0.1f;
    public WeatherManager weatherManager;
    public UnityEngine.Vector2 windForce;
    private Rigidbody2D rb;
    public int mass = 1;
    public float rowingSpeed = 2f; // Speed for the rowing mode
    [Header("-- Camera --")]
    public CameraController cameraController;
    [Header("-- Docking --")]
    public string dockingPointTag = "DockingPoint"; // Tag used to find docking points
    public float maxDockingDistance = 5.0f; // Maximum distance to initiate docking
    // private Transform dockingPoint; // Removed: No longer pre-assigned

    void Start()
    {
        // Ensure animator is assigned (optional if set in Inspector)
        if (boatAnimator == null)
            boatAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        scaledWindSpeed = windSpeed;

        playerMainRenderer = player.GetComponent<SpriteRenderer>();
        playerMovementScript = player.GetComponent<MonoBehaviour>();

        windDirection = GetComponent<WeatherManager>().GetWindDirection();
        windSpeed = GetComponent<WeatherManager>().GetWindSpeed();
    }

    void Update()
    {
        // sailIndicator.SetActive(inBoat);
        // ================================= Movement Checks =================================

        if (inBoat)
        {
            sailIndicator.SetActive(true);
            if (!Compass.activeInHierarchy)
            {
                Compass.SetActive(true);
            }
            scaledWindSpeed = sailsUpScaler * windSpeed;

            // --- DEBUG LOGS ---
            // Debug.Log($"Update: inBoat={inBoat}, anchored={anchored}, isRowingMode={isRowingMode}");

            // --- Independent Input Checks ---

            // 1. Sail Controls (Shift + Arrows)
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shiftPressed)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (sailsUpScaler < 0.99f) sailsUpScaler += 0.01f;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (sailsUpScaler >= 0.01f) sailsUpScaler -= 0.01f;
                }
            }

            // 2. Directional Animation / Sail Angle (Only if Shift is NOT pressed)
            if (!shiftPressed) // Prevent changing direction while adjusting sails
            {
                if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
                {
                    boatAnimator.Play("BoatUpRight");
                    sailAngle = 45f;
                }
                else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
                {
                    boatAnimator.Play("BoatUpLeft");
                    sailAngle = 135f;
                }
                else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
                {
                    boatAnimator.Play("BoatDownRight");
                    sailAngle = 315f;
                }
                else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
                {
                    boatAnimator.Play("BoatDownLeft");
                    sailAngle = 225f;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    boatAnimator.Play("BoatLeft");
                    sailAngle = 180f;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    boatAnimator.Play("BoatRight");
                    sailAngle = 0f;
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    boatAnimator.Play("BoatUp");
                    sailAngle = 90f;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    boatAnimator.Play("BoatDown");
                    sailAngle = 270f;
                }
            }

            // 3. Action Toggles (J, R, T) - Always check these
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("J key pressed - Toggling anchor"); // DEBUG
                anchored = !anchored;
            }

            if (Input.GetKeyDown(KeyCode.R)) // Toggle rowing mode
            {
                Debug.Log("R key pressed - Toggling rowing mode"); // DEBUG
                isRowingMode = !isRowingMode;
                if (isRowingMode) {
                    rb.linearVelocity = UnityEngine.Vector2.zero; // Stop wind momentum
                }
            }

            if (Input.GetKeyDown(KeyCode.T)) // Docking action
            {
                FindAndDockAtClosestPoint();
            }
        } else {
            if (Compass.activeInHierarchy)
            {
                Compass.SetActive(false);
            }
            sailIndicator.SetActive(false);
        }

        //=====================================================================================
    }

    // Sailboat physics 
    void FixedUpdate()
    {
        windDirection = weatherManager.GetWindDirection();
        windSpeed = weatherManager.GetWindSpeed();
        if (inBoat)
        {
            if (anchored) {
                rb.linearVelocity = UnityEngine.Vector2.zero;
            } else {
                
    // Movement logic for the rowing mode
    void HandleRowingMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrows
        float moveVertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down Arrows

        UnityEngine.Vector2 movement = new UnityEngine.Vector2(moveHorizontal, moveVertical);

        // Normalize the movement vector if magnitude > 1 to prevent faster diagonal movement
        if (movement.sqrMagnitude > 1)
        {
            movement.Normalize();
        }

        // Apply the movement directly to the velocity
        rb.linearVelocity = movement * rowingSpeed;

        // Optional: Update animation based on movement direction if needed
        // This part might need adjustment based on your animation setup
        if (movement.magnitude > 0.1f) {
            if (moveVertical > 0.5f) boatAnimator.Play("BoatUp");
            else if (moveVertical < -0.5f) boatAnimator.Play("BoatDown");
            else if (moveHorizontal > 0.5f) boatAnimator.Play("BoatRight");
            else if (moveHorizontal < -0.5f) boatAnimator.Play("BoatLeft");
            // Add diagonal animations if you have them
        } else {
            // Optionally play an idle animation or stop the current one
            // boatAnimator.Play("BoatIdle"); // If you have an idle state
        }
    }
            if (isRowingMode)
            {
                HandleRowingMovement();
            }
            else
            {
                applyForces();
            }
            LockPlayerToBoat();
            }
        }
    }

    // PHYSICS ?!?!?!?!?!!?!?! 
    void applyForces() {
        float sailAngleRad = sailAngle * Mathf.Deg2Rad;

        // Compute normal.
        float sailNormalAngle = sailAngleRad + Mathf.PI / 2f;
        UnityEngine.Vector2 sailNormal = new UnityEngine.Vector2(Mathf.Cos(sailNormalAngle), Mathf.Sin(sailNormalAngle));

        //Compute the wind radians
        float windAngle = Mathf.Atan2(windDirection.y, windDirection.x);

        // Force Magnitude
        float effectiveForceMagnitude = windSpeed * Mathf.Sin(windAngle - sailAngleRad);

        // Scale force
        UnityEngine.Vector2 windForceVector = sailNormal * effectiveForceMagnitude * sailsUpScaler;
        UnityEngine.Vector2 waterResistance = -rb.linearVelocity * waterResistanceFactor;

        windForce = windForceVector + waterResistance;

        //net force
        if (windForce.magnitude > 0.1f) {
            rb.AddForce(windForce);
        }

        if (rb.linearVelocity.magnitude > topSpeed) {
            rb.linearVelocity = rb.linearVelocity.normalized * topSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inBoat = true;
            Debug.Log($"OnTriggerEnter2D: Player entered boat trigger. inBoat={inBoat}, anchored={anchored}"); // DEBUG
            LockPlayerToBoat();
            cameraController.inBoat = true;
            if (Compass != null) 
            {
                Compass.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inBoat = false; // Player is no longer in the boat
            ReleasePlayerFromBoat(); // Release player control
            cameraController.inBoat = false; // Camera should follow player now
            if (Compass != null) Compass.SetActive(false); // Hide compass
        }
    }

    // FUnctions for player when in the boat
    void LockPlayerToBoat()
    {
            if(playerMainRenderer != null)
        {
            playerMainRenderer.enabled = false;
        }

        // Disable movement
        if(playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Lock position
        player.transform.SetParent(transform);
        player.transform.localPosition = playerOffset;
    }

    void ReleasePlayerFromBoat()
    {
        // Re-enable parent renderer
        if(playerMainRenderer != null)
        {
            playerMainRenderer.enabled = true;
            player.transform.SetParent(null);
            if(playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }
        }
    }

    void FindAndDockAtClosestPoint()
    {
        GameObject[] dockingPoints = GameObject.FindGameObjectsWithTag(dockingPointTag);
        Transform closestPoint = null;
        float minDistance = Mathf.Infinity;

        if (dockingPoints.Length == 0)
        {
            Debug.LogWarning($"No GameObjects found with tag '{dockingPointTag}'. Cannot dock.");
            return;
        }

        // Find the closest docking point
        foreach (GameObject pointObject in dockingPoints)
        {
            float distance = UnityEngine.Vector3.Distance(transform.position, pointObject.transform.position); // Specify UnityEngine.Vector3
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = pointObject.transform;
            }
        }

        // Check if the closest point is within range
        if (closestPoint != null && minDistance <= maxDockingDistance)
        {
            DockBoat(closestPoint); // Call DockBoat with the found transform
        }
        else
        {
            Debug.Log("No docking points within range.");
            // Optional: Provide feedback to the player (e.g., UI message)
        }
    }

    void DockBoat(Transform targetDockPoint) // Modified signature
    {
        Debug.Log($"Docking boat at {targetDockPoint.name}...");

        // Ensure targetDockPoint and player are assigned (targetDockPoint is checked before calling)
        if (player == null)
        {
            Debug.LogError("Player Transform not assigned in BoatController Inspector!");
            return; // Exit if essential references are missing
        }

        // Release player control and make them visible again
        ReleasePlayerFromBoat();

        // 1. Teleport the player to exactly targetDockPoint (position and rotation)
        player.transform.position = targetDockPoint.position;

        // 2. Set boat animation based on targetDockPoint's forward direction
        if (boatAnimator != null)
        {
            // Calculate the direction 90 degrees counter-clockwise from docking point's forward (up)
            UnityEngine.Vector3 dockingForward = targetDockPoint.up;
            UnityEngine.Vector3 targetDirection = new UnityEngine.Vector3(-dockingForward.y, dockingForward.x, dockingForward.z);

            SetDockingAnimation(targetDirection);
        }

        // 3. Teleport the boat opposite to the docking point's facing direction
        UnityEngine.Vector3 oppositeDirection = -targetDockPoint.up; // Get the opposite direction
        float boatDistance = 1.0f; // How far behind the dock to place the boat
        UnityEngine.Vector3 boatDockOffset = oppositeDirection * boatDistance;
        transform.position = targetDockPoint.position + boatDockOffset;

        // Stop boat physics movement and anchor it
        rb.linearVelocity = UnityEngine.Vector2.zero;
        rb.angularVelocity = 0f;
        anchored = true;
        isRowingMode = false;

        inBoat = false;
        cameraController.inBoat = false; // Camera follows player
        if (Compass != null) Compass.SetActive(false);
        sailIndicator.SetActive(false);

    }

    // Helper function to set the boat's animation based on a direction vector
    void SetDockingAnimation(UnityEngine.Vector3 direction)
    {
        direction.Normalize();

        string animationToPlay = "BoatDown";

        // Determine the dominant direction (Up, Down, Left, Right)
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0)
            {
                animationToPlay = "BoatUp";
            }
            else
            {
                animationToPlay = "BoatDown";
            }
        }
        else
        {
            if (direction.x > 0)
            {
                animationToPlay = "BoatRight";
            }
            else
            {
                animationToPlay = "BoatLeft";
            }
        }
        boatAnimator.Play(animationToPlay);
    }
}
