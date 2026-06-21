using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer tr;
    private SpriteRenderer sr;
    private Animator animator;
    public GameObject Minigame;

    [Header("Movement")]
    private float moveSpeed = 5.0f;

    private Vector2 moveInput;

    // Dashing
    [Header("Dashing")]
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashForce = 10.0f;

    private bool canDash = true;
    private bool isDashing;

    bool canMove;

    // Animation
    private static readonly int IsSwimmingHash = Animator.StringToHash("IsSwimming");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canMove == true && !Minigame.activeSelf)
        {
            // Get player inputs
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            // Normalize movement
            moveInput.Normalize();

            // Flip sprite to face movement direction 
            if (moveInput.x > 0)
                sr.flipX = false;
            else if (moveInput.x < 0)
                sr.flipX = true;

            if (Input.GetKeyDown(KeyCode.Space) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
        else
        {
            moveInput = Vector2.zero;
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        bool isSwimming = canMove && moveInput.sqrMagnitude > 0.01f;
        animator.SetBool(IsSwimmingHash, isSwimming);
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.AddForce(moveInput * moveSpeed);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        rb.linearVelocity = moveInput * dashForce;
        tr.emitting = true;

        yield return new WaitForSeconds(dashDuration);
        tr.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Water")
        {
            canMove = false;
            rb.gravityScale = 1;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Water")
        {
            canMove = true;
            rb.gravityScale = 0;
        }
    }
}