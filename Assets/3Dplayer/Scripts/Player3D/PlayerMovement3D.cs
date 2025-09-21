using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float fallMultiplier = 3.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallJumpForce = 7f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpInputBuffer = 0.2f;

    [Header("References")]
    [SerializeField] private Transform visuals;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource jumpAudio;

    [Header("Particles")]
    [SerializeField] private ParticleSystem wallSlideParticles;
    [SerializeField] private Vector3 wallSlideOffset = new Vector3(0.5f, -0.5f, 0f);

    private Rigidbody rb;
    private bool isGrounded;
    private float moveX;

    // Wall jump tracking
    private bool isTouchingWall;
    private bool wallJumped;
    private float jumpBufferTime;
    private Transform lastWall;

    // --- Win/Lose states ---
    public bool HasWon { get; private set; } = false;
    public bool HasLost { get; private set; } = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (HasWon || HasLost) return;
        if (visuals == null || animator == null || groundCheck == null) return;

        moveX = Input.GetAxisRaw("Horizontal");

        // --- Jump input buffer ---
        if (Input.GetButtonDown("Jump"))
            jumpBufferTime = wallJumpInputBuffer;
        else
            jumpBufferTime -= Time.deltaTime;

        // --- Ground check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // --- Wall check ---
        RaycastHit hitRight, hitLeft;
        bool hitWallRight = Physics.Raycast(transform.position, Vector3.right, out hitRight, wallCheckDistance, wallMask);
        bool hitWallLeft = Physics.Raycast(transform.position, Vector3.left, out hitLeft, wallCheckDistance, wallMask);
        isTouchingWall = hitWallRight || hitWallLeft;

        Transform currentWall = null;
        if (hitWallRight) currentWall = hitRight.collider.transform;
        else if (hitWallLeft) currentWall = hitLeft.collider.transform;

        // Reset wall jump if grounded or touching a new wall
        if (isGrounded)
            wallJumped = false;
        else if (currentWall != null && currentWall != lastWall)
            wallJumped = false;

        lastWall = currentWall;

        // --- Handle jumps ---
        if (jumpBufferTime > 0)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
                PlayJumpSound();
                jumpBufferTime = 0;
            }
            else if (isTouchingWall && !wallJumped)
            {
                wallJumped = true;
                jumpBufferTime = 0;

                float direction = hitWallRight ? -1 : 1;

                rb.velocity = new Vector3(direction * wallJumpDirection.x * wallJumpForce,
                                          wallJumpDirection.y * wallJumpForce, 0);

                visuals.localScale = new Vector3(direction, 1, 1);

                PlayJumpSound();
            }
        }

        // --- Sprite flip ---
        if (moveX > 0.01f)
            visuals.localScale = new Vector3(1, 1, 1);
        else if (moveX < -0.01f)
            visuals.localScale = new Vector3(-1, 1, 1);

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (HasWon || HasLost) return;

        Vector3 velocity = rb.velocity;

        // --- Horizontal movement ---
        velocity.x = moveX * speed;
        velocity.z = 0;

        // --- Wall slide ---
        if (!isGrounded && isTouchingWall && rb.velocity.y < 0 && !wallJumped)
        {
            velocity.y = -wallSlideSpeed;

            if (wallSlideParticles != null)
            {
                if (!wallSlideParticles.isPlaying)
                    wallSlideParticles.Play();

                // Determine wall side
                float direction = Physics.Raycast(transform.position, Vector3.right, wallCheckDistance, wallMask) ? 1 : -1;

                // Update particle position relative to player
                wallSlideParticles.transform.localPosition = new Vector3(direction * Mathf.Abs(wallSlideOffset.x),
                                                                        wallSlideOffset.y,
                                                                        wallSlideOffset.z);

                // Flip particle rotation to face the correct direction
                var main = wallSlideParticles.main;
                main.startRotationY = direction == 1 ? 0f : Mathf.PI; // rotate 180° if left wall
            }
        }
        else
        {
            if (wallSlideParticles != null && wallSlideParticles.isPlaying)
                wallSlideParticles.Stop();
        }

        // --- Custom gravity ---
        if (velocity.y < 0)
            velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
            velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;

        rb.velocity = velocity;
    }

    private void UpdateAnimations()
    {
        if (HasWon)
        {
            animator.Play("Win");
            return;
        }

        if (HasLost)
        {
            animator.Play("Idle");
            return;
        }

        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);
            if (Mathf.Abs(moveX) > 0.01f)
                animator.Play("Walk");
            else
                animator.Play("Idle");
        }
        else
        {
            animator.SetBool("isGrounded", false);
            if (rb.velocity.y > 0)
                animator.Play("Jump");
            else
                animator.Play("Fall");
        }

        animator.SetFloat("Speed", Mathf.Abs(moveX));
    }

    private void PlayJumpSound()
    {
        if (jumpAudio != null)
        {
            jumpAudio.pitch = Random.Range(0.95f, 1.05f);
            jumpAudio.Play();
        }
    }

    public void TriggerWin()
    {
        HasWon = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetBool("HasWon", true);
    }

    public void TriggerLose()
    {
        HasLost = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetBool("HasLost", true);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        if (wallCheckDistance > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        }
    }
}