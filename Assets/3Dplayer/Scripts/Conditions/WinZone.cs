using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WinZone : MonoBehaviour
{
    [Header("Win Settings")]
    [SerializeField] private string winMessage = "You Win!"; // Message shown in console

    private void OnTriggerEnter(Collider other)
    {
        // Debug log to check trigger
        Debug.Log("Trigger entered by: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log(winMessage);

            // Stop player movement
            PlayerMovement3D playerMovement = other.GetComponent<PlayerMovement3D>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Stop any ongoing physics movement
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }
}
