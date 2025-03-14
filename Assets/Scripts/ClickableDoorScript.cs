using UnityEngine;
using UnityEngine.SceneManagement;
public class ClickableDoorScript : MonoBehaviour
{
    public string sceneToLoad;
    public bool isClickable = false;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>(); // Get the object's collider
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (col.OverlapPoint(mousePos))
        {
            isClickable = true;
        } else {
            isClickable = false;
        }

        if (Input.GetMouseButtonDown(0) && isClickable)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
