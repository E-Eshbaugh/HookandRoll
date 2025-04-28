using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElevationEntry : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;

    public bool height = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            Debug.Log("hit player entry");
            if (height)
            {
                height = !height;
                foreach (var mountain in mountainColliders)
                {
                    mountain.enabled = true;
                }

                foreach (var boundary in boundaryColliders)
                {
                    boundary.enabled = false;
                }
            } else {
                height = !height;
                foreach (var mountain in mountainColliders)
                {
                    mountain.enabled = false;
                }

                foreach (var boundary in boundaryColliders)
                {
                    boundary.enabled = true;
                }
            }


            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
        }
    }
}
