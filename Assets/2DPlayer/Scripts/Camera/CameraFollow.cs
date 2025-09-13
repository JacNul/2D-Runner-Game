using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;   // Player to follow
    [SerializeField] private float smoothSpeed = 0.2f; // Camera follow smoothing
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f); // Camera offset
    [SerializeField] private float zoomOutSize = 7f; // Bigger number = zoomed out

    [Header("Camera Options")]
    [SerializeField] private bool lockY = true; // Lock Y like Shovel Knight

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float fixedY; // Stores Y position if lockY is enabled

    private void Awake()
    {
        cam = GetComponent<Camera>();
        fixedY = transform.position.y; // Save initial Y
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Target position
        Vector3 targetPos = player.position + offset;

        if (lockY)
        {
            targetPos.y = fixedY; // Freeze Y to starting camera height
        }

        // Smooth damp follow
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);

        // Zoom out
        cam.orthographicSize = zoomOutSize;
    }
}
