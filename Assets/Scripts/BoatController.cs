using System;
using System.Numerics;
using UnityEditor.Callbacks;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Animator boatAnimator;
    public Boolean inBoat = false;
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
    [Header("-- Camera --")]
    public CameraController cameraController;
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
        // ================================= Movement Checks =================================
        if (inBoat)
        {
            scaledWindSpeed = sailsUpScaler * windSpeed;

            if (Input.GetKey(KeyCode.UpArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {   
                if (sailsUpScaler < 0.99f)
                {
                    sailsUpScaler += 0.01f;
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (sailsUpScaler >= 0.01f)
                {
                sailsUpScaler -= 0.01f;
                }
            }
            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
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

        //=====================================================================================
    }

    // Sailboat physics 
    void FixedUpdate()
    {
        windDirection = weatherManager.GetWindDirection();
        windSpeed = weatherManager.GetWindSpeed();
        if (inBoat)
        {
            applyForces();
            LockPlayerToBoat();
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
        rb.AddForce(windForce);

        if (rb.linearVelocity.magnitude > topSpeed) {
            rb.linearVelocity = rb.linearVelocity.normalized * topSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inBoat = true;
            LockPlayerToBoat();
            cameraController.inBoat = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //inBoat = false;
            //cameraController.inBoat = true;
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

//  ---------------- Hasnt been implemented and havent tested yet --------------------
    void ReleasePlayerFromBoat()
    {
        // Re-enable parent renderer
        if(playerMainRenderer != null)
        {
            playerMainRenderer.enabled = true;
        }

        // Re-enable movement
        if(playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        // Unparent while maintaining world position
        player.transform.SetParent(null);
    }
}
