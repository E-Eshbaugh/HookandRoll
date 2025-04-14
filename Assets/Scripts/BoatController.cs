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
    public float sailAngle = 180f;
    public float windSpeed = 10f;
    public float scaledWindSpeed;
    public float windDirection = 180f;
    public float sailsUpScaler = 0.0f;
    public float topSpeed = 10f;
    public float waterResistanceFactor = 0.1f;
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
                sailAngle = 225f;
            }
            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                boatAnimator.Play("BoatUpLeft");
                sailAngle = 315f;
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
                sailAngle = 180f;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                boatAnimator.Play("BoatUp");
                sailAngle = 270f;
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
        if (inBoat)
        {
            LockPlayerToBoat();
            float sailNormalAngle = (sailAngle + 90f) * Mathf.Deg2Rad;
            UnityEngine.Vector2 sailNormal = new UnityEngine.Vector2(Mathf.Cos(sailNormalAngle), Mathf.Sin(sailNormalAngle));

            float windAngleRad = windDirection * Mathf.Deg2Rad;
            UnityEngine.Vector2 windVector = new UnityEngine.Vector2(Mathf.Cos(windAngleRad), Mathf.Sin(windAngleRad)) * windSpeed;

            float dotProduct = UnityEngine.Vector2.Dot(windVector.normalized, sailNormal);
            float effectiveForce = Mathf.Max(dotProduct, 0) * windSpeed * sailsUpScaler;

            windForce = sailNormal * effectiveForce;

            UnityEngine.Vector2 waterResistance = -rb.linearVelocity * waterResistanceFactor;

            rb.AddForce(windForce + waterResistance);

            if (rb.linearVelocity.magnitude > topSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * topSpeed;
            }
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
