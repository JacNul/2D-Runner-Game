using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    //Movement Speed and Jump
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    //Ground Check for player
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    //Audio
    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;

    //Animation Render
    [Header("Animation")]
    [SerializeField] private Animator animator; // Assign your Animator here
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign the player's sprite renderer

    private Rigidbody2D body;
    private AudioSource audioSource;
    private bool isFacingRight = true;

    private void Awake()
    {
        //Freeze body rotation
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;

        //Jump Audio + Jump Animation
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Horizontal movement
        float move = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(move * speed, body.velocity.y);

        // Flip sprite
        if (move > 0 && !isFacingRight)
            Flip();
        else if (move < 0 && isFacingRight)
            Flip();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);

            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // Update animations
        UpdateAnimations(move);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        //Horizontal sprite flip
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void UpdateAnimations(float move)
    {
        bool isGrounded = IsGrounded();

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", body.velocity.y);
    }

    //Ground Check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}