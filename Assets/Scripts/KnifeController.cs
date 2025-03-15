using Unity.VisualScripting;
using UnityEngine;

public class ObjectClickMove : MonoBehaviour
{
    private bool isDragging = false;
    private Collider2D col;
    public bool canPickup = false;
    public Animator animator;

    void Start()
    {
        col = GetComponent<Collider2D>(); // Get the object's collider
    }

    void Update()
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (col.OverlapPoint(mousePos))
        {
            canPickup = true;
        } else {
            canPickup = false;
        }

        if (isDragging)
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
            if (Input.GetMouseButtonDown(0)){
                animator.SetTrigger("Click");
            }
            
        }

        if (Input.GetMouseButtonDown(0) && canPickup)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject && !isDragging)
            {
                isDragging = true;
                animator.SetTrigger("PickedUp");
            }
        }
    }
}
