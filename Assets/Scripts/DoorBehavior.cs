using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorBehavior : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosistion;
    public VectorValue playerInitialPos;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            playerInitialPos.initialValue = playerPosistion;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
