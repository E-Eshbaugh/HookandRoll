using UnityEngine;
using UnityEngine.UI; 

public class SailManager : MonoBehaviour
{
    [Header("--- Sail Sprites ---")]
    public Sprite sail_down;
    public Sprite sail_up;
    public Sprite sail_25;
    public Sprite sail_50;
    public Sprite sail_75;
    public MonoBehaviour boat;
    public Image targetImage;
    public float sailAmount = 0.0f;
    private bool inBoat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        inBoat = boat.GetComponent<BoatController>().inBoat;

        sailAmount = boat.GetComponent<BoatController>().sailsUpScaler;

        if (sailAmount <= 0.01f)
        {
            targetImage.sprite = sail_down;
        }
        else if (sailAmount > 0.0f && sailAmount < 0.25f)
        {
            targetImage.sprite = sail_25;
        }
        else if (sailAmount >= 0.25f && sailAmount < 0.5f)
        {
            targetImage.sprite = sail_50;
        }
        else if (sailAmount >= 0.5f && sailAmount < 0.75f)
        {
            targetImage.sprite = sail_75;
        }
        else if (sailAmount >= 0.75f && sailAmount <= 1.0f)
        {
            targetImage.sprite = sail_up;
        }
    }
}
