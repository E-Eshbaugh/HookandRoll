using UnityEngine;

public class RainController : MonoBehaviour
{
    // Reference to the rain object
    public GameObject rainObject;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;

    // Start with rain off (optional)
    void Start()
    {
        if (rainObject != null)
        {
            rainObject.SetActive(false);
        }
    }

    // Function to enable or disable the rain
    public void ToggleRain(bool isActive)
    {
        if (rainObject != null)
        {
            rainObject.SetActive(isActive);
        }
    }

    // Optional: Toggle rain using a key (e.g., R key)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (rainObject != null)
            {
                rainObject.SetActive(!rainObject.activeSelf);
                if (rainObject.activeSelf)
                {
                    musicSource.clip = background;
                    musicSource.Play(0);
                } else
                {
                    musicSource.Stop();
                }
            }

        }
    }
}
