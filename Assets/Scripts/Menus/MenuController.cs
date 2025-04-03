using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    bool isPaused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (isPaused) {
                Time.timeScale = 1.0f;
                pauseMenu.SetActive(false);
                isPaused = false;
            } else {
                Time.timeScale = 0.0f;
                pauseMenu.SetActive(true);
                isPaused = true;
            }
        }
    }
}
