using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TMPro;

public class LoseZone : MonoBehaviour
{
    [Header("Lose Settings")]
    [SerializeField] private string loseMessage = "You Lose!";
    [SerializeField] private GameObject loseTextObject; // Drag UI Text here in Inspector

    private void Start()
    {
        if (loseTextObject != null)
            loseTextObject.SetActive(false); // Hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(loseMessage);

            PlayerMovement3D player = other.GetComponent<PlayerMovement3D>();
            if (player != null)
            {
                player.TriggerLose();
            }

            if (loseTextObject != null)
            {
                loseTextObject.SetActive(true);
                var text = loseTextObject.GetComponent<TMP_Text>();
                if (text != null) text.text = loseMessage;
            }
        }
    }
}