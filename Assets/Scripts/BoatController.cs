using System;
using System.Numerics;
using UnityEditor.Callbacks;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Animator boatAnimator;
    public Boolean inBoat = false;
    public float sailAngle = 180f;
    public float windSpeed = 10f;
    public float scaledWindSpeed;
    public float windDirection = 180f;
    public float sailsUpScaler = 0.0f;
    public UnityEngine.Vector2 windForce;
    private Rigidbody2D rb;
    public int mass = 1;
    public CameraController cameraController;
    void Start()
    {
        // Ensure animator is assigned (optional if set in Inspector)
        if (boatAnimator == null)
            boatAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        scaledWindSpeed = windSpeed;
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

    void FixedUpdate()
    {
        if (inBoat) {
            float angleDifference = (windDirection - sailAngle) * Mathf.Deg2Rad;
            float sinDiff = Mathf.Sin(angleDifference);

            float forceMagnitude = scaledWindSpeed * scaledWindSpeed / mass * sinDiff;

            float perpAngle = (sailAngle + 90f) * Mathf.Deg2Rad;
            UnityEngine.Vector2 forceDirection = new UnityEngine.Vector2(Mathf.Cos(perpAngle), Mathf.Sin(perpAngle));
            windForce = forceMagnitude * forceDirection;
            rb.AddForce(windForce);

            Debug.DrawLine(transform.position, transform.position + (UnityEngine.Vector3)windForce, Color.red);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inBoat = true;
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
}
