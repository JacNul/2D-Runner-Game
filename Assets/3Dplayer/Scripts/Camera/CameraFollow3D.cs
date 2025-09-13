using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow3D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoom = 5f; // Camera distance / size
    [SerializeField] private bool orthographic = true; // Set based on camera type

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = orthographic;
        ApplyZoom();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply zoom every frame in case you tweak it in Inspector
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        if (cam == null) return;

        if (orthographic)
            cam.orthographicSize = zoom;
        else
            cam.fieldOfView = zoom;
    }
}
