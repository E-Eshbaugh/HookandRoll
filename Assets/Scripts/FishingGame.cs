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

    public bool onFish = false;
    public fishing playerS;
    public GameObject bobber;

    public Transform player;
    public float timeChangeRate = 0.25f;
    [SerializeField] private Collider2D indicatorCollider;
    private Collider2D barCollider;


    void Start()
    {
        barCollider = GetComponent<Collider2D>();
    }

    void CheckCollision()
    {
        onFish = barCollider.bounds.Intersects(indicatorCollider.bounds);
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();

        if(onFish == true)
        {
            targetTime += timeChangeRate * Time.deltaTime;;
        }
        if (onFish == false)
        {
            targetTime -= timeChangeRate * Time.deltaTime;;
        }

         if(targetTime <= 0.0f)
         {
             transform.localPosition = new Vector3(-0.129f, -0.919f, 0);
             onFish = false;
             playerS.fishGameLost();
             Destroy(GameObject.Find("bobber(Clone)"));
             targetTime = 2.0f;
         }
         if (targetTime >= 4.5f)
         {
             transform.localPosition = new Vector3(-0.129f, -0.919f, 0);
             onFish = false;
             playerS.fishGameWon();
             Destroy(GameObject.Find("bobber(Clone)"));
             targetTime = 2.0f;
         }

         if(targetTime >= 0.0f)
         {
             p1.SetActive(false);
             p2.SetActive(false);
             p3.SetActive(false);
             p4.SetActive(false);
             p5.SetActive(false);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 0.5f)
         {
             p1.SetActive(true);
             p2.SetActive(false);
             p3.SetActive(false);
             p4.SetActive(false);
             p5.SetActive(false);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 1.0f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(false);
             p4.SetActive(false);
             p5.SetActive(false);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 1.5f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(false);
             p5.SetActive(false);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 2.0f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(true);
             p5.SetActive(false);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 2.5f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(true);
             p5.SetActive(true);
             p6.SetActive(false);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 3.0f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(true);
             p5.SetActive(true);
             p6.SetActive(true);
             p7.SetActive(false);
             p8.SetActive(false);
         }
         if (targetTime >= 3.5f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(true);
             p5.SetActive(true);
             p6.SetActive(true);
             p7.SetActive(true);
             p8.SetActive(false);
         }
         if (targetTime >= 4.0f)
         {
             p1.SetActive(true);
             p2.SetActive(true);
             p3.SetActive(true);
             p4.SetActive(true);
             p5.SetActive(true);
             p6.SetActive(true);
             p7.SetActive(true);
             p8.SetActive(true);
         }

        Vector2 rbCoords = rb.position;
        Vector2 playerPosition = player.position;
        Camera cam = Camera.main;
        Vector2 screenTop = cam.ScreenToWorldPoint(new Vector2(0, Screen.height));
        Vector2 screenCenter = cam.ScreenToWorldPoint(new Vector2(0, Screen.height/2));
        float worldScreenHeight = screenTop.y - screenCenter.y;
        float travelDist = worldScreenHeight * 0.2f;

        // Normalized forces
        float baseForce = 70.0f;
        float normalizedForce = baseForce * Time.fixedDeltaTime;

        if (rbCoords.y < playerPosition.y - travelDist)
        {
            atTop = false;
        }
        else if (rbCoords.y > playerPosition.y + travelDist)
        {
            atTop = true;
            rb.AddForce(Vector2.down * normalizedForce);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * normalizedForce);
            spaceBar = true;
        }
        else
        {
            spaceBar = false;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            playerS.fishGameLost();
            // Reset game state
            onFish = false;
            targetTime = savedTargetTime;
        }
    }  
    
}