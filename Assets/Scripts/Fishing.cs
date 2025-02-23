using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    // Start is called before the first frame update
    //public Animator playerAnim;
    public bool isFishing;
    public bool poleBack;
    public bool throwBobber;
    //public Transform fishingPoint;
    //public GameObject bobber;

    public float targetTime = 0.0f;
    public float savedTargetTime;
    public float extraBobberDistance;

    //public GameObject fishGame;

    public float timeTillCatch = 0.0f;
    //public bool winnerAnim;

    void Start()
    {
        isFishing = false;
        //fishGame.SetActive(false);
        throwBobber = false;
        targetTime = 0.0f;
        savedTargetTime = 0.0f;
        extraBobberDistance = 0.0f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isFishing == false)
        {
            poleBack = true;
            print("We fishing");
        }
    }
}
