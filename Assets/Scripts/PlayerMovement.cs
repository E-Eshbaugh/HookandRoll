using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    public GameObject boat;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    public VectorValue startPosition;
    public string targetSceneName = "inside";
    public SpriteRenderer spriteRenderer;
    public bool playerInBoat = false;
    public Vector2 externalVelocity = Vector2.zero; // Velocity applied by external forces like conveyors
    private Transform boatTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boatTransform = boat.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        transform.position = startPosition.initialValue;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (boat.GetComponent<BoatController>().inBoat)
        {
            playerInBoat = true;
            rb.linearVelocity = Vector2.zero;
            animator.Play("Idle");
        }
        else
        {
            playerInBoat = false;
        }

        rb.linearVelocity = (moveInput * moveSpeed) + externalVelocity;

        if (SceneManager.GetActiveScene().name == targetSceneName)
        {
            if (spriteRenderer != null)
            {

                spriteRenderer.transform.localScale *= 2;
                Debug.Log("Sprite size doubled in scene: " + targetSceneName);
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("IsWalking", true);

        if(context.canceled)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    public Vector2 GetLastInputDirection()
    {
        Vector2 lastInput = new Vector2(animator.GetFloat("LastInputX"), animator.GetFloat("LastInputY"));
        if(lastInput == Vector2.zero)
        {
            lastInput = Vector2.up;
        }
        return lastInput.normalized;
    }

}
