using System;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Animator boatAnimator;
    public Boolean inBoat = false;

    void Start()
    {
        // Ensure animator is assigned (optional if set in Inspector)
        if (boatAnimator == null)
            boatAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // ================================= Movement Checks =================================
        if (inBoat)
        {
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                boatAnimator.Play("BoatUpRight");
            }
            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                boatAnimator.Play("BoatUpLeft");
            }
            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                boatAnimator.Play("BoatDownRight");
            }
            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                boatAnimator.Play("BoatDownLeft");
            }

            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                boatAnimator.Play("BoatLeft");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                boatAnimator.Play("BoatRight");
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                boatAnimator.Play("BoatUp");
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                boatAnimator.Play("BoatDown");
            }
        }

        //=====================================================================================


    }
}
