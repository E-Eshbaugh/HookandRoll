using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishingGame : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rb;
    public bool atTop;
    public float targetTime = 4.0f;
    public float savedTargetTime;
    public bool spaceBar = false;

    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject p5;
    public GameObject p6;
    public GameObject p7;
    public GameObject p8;

    public bool onFish;
    public fishing playerS;
    public GameObject bobber;

    public Transform player;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(onFish == true)
        {
            targetTime += Time.deltaTime;
        }
        if (onFish == false)
        {
            targetTime -= Time.deltaTime;
        }

        // if(targetTime <= 0.0f)
        // {
        //     transform.localPosition = new Vector3(-0.129f, -0.919f, 0);
        //     onFish = false;
        //     playerS.fishGameLost();
        //     Destroy(GameObject.Find("bobber(Clone)"));
        //     targetTime = 4.0f;
        // }
        // if (targetTime >= 8.0f)
        // {
        //     transform.localPosition = new Vector3(-0.129f, -0.919f, 0);
        //     onFish = false;
        //     playerS.fishGameWon();
        //     Destroy(GameObject.Find("bobber(Clone)"));
        //     targetTime = 4.0f;
        // }

        // if(targetTime >= 0.0f)
        // {
        //     p1.SetActive(false);
        //     p2.SetActive(false);
        //     p3.SetActive(false);
        //     p4.SetActive(false);
        //     p5.SetActive(false);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 1.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(false);
        //     p3.SetActive(false);
        //     p4.SetActive(false);
        //     p5.SetActive(false);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 2.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(false);
        //     p4.SetActive(false);
        //     p5.SetActive(false);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 3.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(false);
        //     p5.SetActive(false);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 4.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(true);
        //     p5.SetActive(false);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 5.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(true);
        //     p5.SetActive(true);
        //     p6.SetActive(false);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 6.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(true);
        //     p5.SetActive(true);
        //     p6.SetActive(true);
        //     p7.SetActive(false);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 7.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(true);
        //     p5.SetActive(true);
        //     p6.SetActive(true);
        //     p7.SetActive(true);
        //     p8.SetActive(false);
        // }
        // if (targetTime >= 8.0f)
        // {
        //     p1.SetActive(true);
        //     p2.SetActive(true);
        //     p3.SetActive(true);
        //     p4.SetActive(true);
        //     p5.SetActive(true);
        //     p6.SetActive(true);
        //     p7.SetActive(true);
        //     p8.SetActive(true);
        // }
        Vector2 rbCoords = rb.position;
        Vector2 playerPosition = player.position; // Reference to the player's position
        Camera cam = Camera.main;
        Vector2 screenTop = cam.ScreenToWorldPoint(new Vector2(0, Screen.height));
        Vector2 screenCenter = cam.ScreenToWorldPoint(new Vector2(0, Screen.height/2));
        float worldScreenHeight = screenTop.y - screenCenter.y;
        float travelDist = worldScreenHeight * 0.2f;

        // Debug current positions

        // Use player's Y position as the reference point instead of a fixed height
        // Apply stronger upward force when below the limit
        if (rbCoords.y < playerPosition.y - travelDist)
        {
            atTop = true;
            // Apply a stronger force when far below the limit
            float distanceFactor = Mathf.Abs(rbCoords.y - (playerPosition.y - travelDist)) + 1;
            Vector2 upForce = Vector2.up * distanceFactor * 2f;
            rb.AddForce(upForce); // Push upward with increased force
            
            // Immediately set a minimum upward velocity to ensure it moves up
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, 1.0f));
        }
        // Check upper limit only if not currently recovering from being below
        else if (rbCoords.y > playerPosition.y + travelDist)
        {
            atTop = false;
            rb.AddForce(Vector2.down); // Push downward when above bounds
            // Stop from moving up any more
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 0));
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector2.up*(2.0f));
            spaceBar = true;
        } else {
            spaceBar = false;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            playerS.fishGameLost();
            // Reset game state
            onFish = false;
            targetTime = savedTargetTime;
            gameObject.SetActive(false);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("fish"))
        {
            onFish = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("fish"))
        {
            onFish = false;
        }
    }   
}