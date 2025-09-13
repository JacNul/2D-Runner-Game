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

    [Header("References")]
    [SerializeField] private Transform visuals;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource jumpAudio;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveX;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (visuals == null || animator == null || groundCheck == null) return;

        // --- Input ---
        moveX = Input.GetAxisRaw("Horizontal");

        // --- Ground check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
            if (jumpAudio != null)
            {
                jumpAudio.pitch = Random.Range(0.95f, 1.05f);
                jumpAudio.Play();
            }
        }

        // --- Sprite flip ---
        if (moveX > 0.01f)
            visuals.localScale = new Vector3(1, 1, 1);
        else if (moveX < -0.01f)
            visuals.localScale = new Vector3(-1, 1, 1);

        // --- Update animations ---
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // --- Horizontal movement ---
        Vector3 velocity = rb.velocity;
        velocity.x = moveX * speed;
        velocity.z = 0;
        rb.velocity = velocity;

        // --- Custom gravity ---
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void UpdateAnimations()
    {
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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}