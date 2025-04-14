using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class fishing : MonoBehaviour
{
    public Animator playerAnim;
    private GameObject spawnBobber;
    public Rigidbody2D boat;
    public bool isFishing;
    public bool poleBack;
    public bool throwBobber;
    private Transform fishingPoint;
    public GameObject bobber;
    public GameObject target;
    public Rigidbody2D player;

    public GameObject fishingTarget;

    public float targetTime = 0.0f;
    public float savedTargetTime;
    public float extraBobberDistance;

    public GameObject fishGame;

    public float timeTillCatch = 0.0f;
    public bool winnerAnim;
    public float scaleSpeed = 0.5f;
    public float fishingRange = 20.0f;
    private float targetScalingDirection = 1;
    private bool clickTime = false;
    private bool canRange = true;
    private Vector3 mousePos;
    private bool freeze = false;
    void Start()
    {
        isFishing = false;
        fishGame.SetActive(false);
        target.SetActive(false);
        throwBobber = false;
        targetTime = 0.0f;
        savedTargetTime = 0.0f;
        extraBobberDistance = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze) {
            player.constraints = RigidbodyConstraints2D.FreezeAll;
        } else {
            player.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if(Input.GetKey(KeyCode.Space) && isFishing == false && winnerAnim == false && canRange == true && boat.linearVelocity.magnitude <= 0.1f)
        {
            freeze = true;
            fishingTarget.SetActive(true);
            poleBack = true;
            fishingTarget.transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime * targetScalingDirection;
            if (fishingTarget.transform.localScale.x >= fishingRange)
            {
                targetScalingDirection = -1;
            }
            else if (fishingTarget.transform.localScale.x <= 0.3f)
            {
                targetScalingDirection = 1;
            }
        }

        if(isFishing == true)
        {
            timeTillCatch += Time.deltaTime;
            if(timeTillCatch >= 3)
            {
                fishGame.SetActive(true);
            }
        }


        if(Input.GetKeyUp(KeyCode.Space) && isFishing == false && winnerAnim == false && boat.linearVelocity.magnitude <= 0.1f)
        {
            target.SetActive(true);
            clickTime = true;
            poleBack = false;
            canRange = false;
        }

        if (Input.GetMouseButtonDown(0) && target.activeSelf && isFishing == false && clickTime == true)
        {

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 10;
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == fishingTarget)
            {
                spawnBobber = Instantiate(bobber, mousePos, Quaternion.identity);
                isFishing = true;
                target.SetActive(false);
                playerAnim.Play("Fishing");
                fishGame.SetActive(true);
                canRange = true;
            }
            else
            {
                target.SetActive(false);
                isFishing = false;
                canRange = true;
                fishGameLost();
            }
            clickTime = false;
        }

        //fishingPoint.transform.position = mousePos;

        if(poleBack == true)
        {
            playerAnim.Play("SwingBack");
            savedTargetTime = targetTime;
            targetTime += Time.deltaTime;
        }

        if(isFishing == true)
        {
            playerAnim.Play("Fishing");
        }

        if(Input.GetKeyDown(KeyCode.P) && timeTillCatch <= 3)
        {
            playerAnim.Play("Idle");
            poleBack = false;
            throwBobber = false;
            isFishing = false;
            timeTillCatch = 0;
        }

    }

    public void fishGameWon()
    {
        Destroy(spawnBobber);
        playerAnim.Play("WonFish");
        fishGame.SetActive(false);
        poleBack = false;
        isFishing = false;
        throwBobber = false;
        timeTillCatch = 0;
        fishingTarget.SetActive(false);
    } 
    public void fishGameLost()
    {
        Destroy(spawnBobber);
        playerAnim.Play("Idle");
        fishGame.SetActive(false);
        poleBack = false;
        throwBobber = false;
        isFishing = false;
        timeTillCatch = 0;
        fishingTarget.SetActive(false);
        freeze = false;
    }


}