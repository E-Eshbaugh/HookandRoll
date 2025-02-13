using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [Header("Hook Settings")]
    public GameObject fishingHookPrefab;  // Drag your hook prefab here.
    public float castDistanceMultiplier = 5f; // How far the hook goes per unit charge.
    public float maxCharge = 3f;              // Maximum time you can charge.
    public float chargeSpeed = 1f;            // How fast the charge accumulates.

    [Header("FOV Settings")]
    public float fovAngle = 60f; // Total angle (in degrees) of the field-of-view in front of the player.

    [Header("Line Renderer")]
    public LineRenderer lineRenderer; // Assign a LineRenderer component.

    private float currentCharge = 0f;
    private bool isCharging = false;
    private bool isHookCast = false;
    private GameObject currentHook;
    private PlayerMovement playerMovement;


    void Awake() 
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Start charging or retract if hook is already out.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isHookCast)
            {
                RetractHook();
            }
            else
            {
                isCharging = true;
                currentCharge = 0f;
            }
        }

        // Accumulate charge while space is held.
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
            // Future: Visualize charge here (update a UI bar)
        }

        // On releasing space (and if not already cast) perform the cast.
        if (Input.GetKeyUp(KeyCode.Space) && isCharging && !isHookCast)
        {
            CastHook();
            isCharging = false;
        }

        // If hook is out, update the line renderer to draw the line between the player and hook.
        if (isHookCast && currentHook != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentHook.transform.position);
        }
    }

    void CastHook()
    {
        // Determine the player's forward direction.
        Vector3 forward = transform.up;

        // Calculate a random offset angle within half the FOV on either side.
        float halfFOV = fovAngle / 2f;
        float angleOffset = Random.Range(-halfFOV, halfFOV);

        Vector2 castDirection = playerMovement.GetLastInputDirection();
        // Use castDirection to determine the target for fishing line
        float castDistance = currentCharge * castDistanceMultiplier;
        Vector3 targetPosition = transform.position + (Vector3)(castDirection * castDistance);

        // Instantiate the hook prefab.
        currentHook = Instantiate(fishingHookPrefab, transform.position, Quaternion.identity);
        
        // Instantly move hook to final position
        currentHook.transform.position = targetPosition;

        isHookCast = true;

        // Set up the line renderer.
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentHook.transform.position);
        }
    }

    void RetractHook()
    {
        if (currentHook != null)
        {
            Destroy(currentHook);
        }
        isHookCast = false;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}