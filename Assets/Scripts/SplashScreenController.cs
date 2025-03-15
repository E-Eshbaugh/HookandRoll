using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public GameObject flashingText;

    private float timer = 0.5f;
    private float interval = 0.5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("startScene");
        }

        timer += Time.deltaTime;

        // Check if the interval has passed
        if (timer >= interval)
        {
            if (flashingText.activeSelf)
            {
                flashingText.SetActive(false); // Hide the text
            }
            else
            {
                flashingText.SetActive(true); // Show the text
            }
            timer = 0f;
        }
    }
}
