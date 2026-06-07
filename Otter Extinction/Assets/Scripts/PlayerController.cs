using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer tr;

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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get player inputs
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        // Normalize movement
        moveInput.Normalize();

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
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
        
        // add velocity direction pressed+ dashforce
        rb.linearVelocity = moveInput * dashForce;
        tr.emitting = true;

        yield return new WaitForSeconds(dashDuration);
        tr.emitting = false;
        isDashing = false;
        
        // Cooldown before dashing again
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
