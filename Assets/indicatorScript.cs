using UnityEngine;

public class indicatorScript : MonoBehaviour
{
    public float timer = 0f; // The current time
    public float timeChangeRate = 1f; // Rate of increase/decrease per second
    public bool isInside = false; // To track if the indicator is inside

    void Update()
    {
        if (isInside)
        {
            timer += timeChangeRate * Time.deltaTime; // Count up
        }
        else
        {
            timer -= timeChangeRate * Time.deltaTime; // Count down
        }

        // Ensure timer doesn't go below 0
        timer = Mathf.Max(0, timer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bar")) // Make sure the indicator has this tag
        {
            isInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("bar"))
        {
            isInside = false;
        }
    }
}


