using UnityEngine;

public class ElevationExit : MonoBehaviour
{
        public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            foreach (var mountain in mountainColliders)
            {
                mountain.enabled = true;
            }

            foreach (var boundary in boundaryColliders)
            {
                boundary.enabled = false;
            }

            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
    }
}
