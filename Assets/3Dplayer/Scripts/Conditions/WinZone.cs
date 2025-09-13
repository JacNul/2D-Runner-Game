using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TMPro; // Needed if using TextMeshPro

public class WinZone : MonoBehaviour
{
    [Header("Win Settings")]
    [SerializeField] private string winMessage = "You Win!";
    [SerializeField] private GameObject winTextObject; // Drag UI Text here in Inspector

    private void Start()
    {
        if (winTextObject != null)
            winTextObject.SetActive(false); // Hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(winMessage);

            PlayerMovement3D player = other.GetComponent<PlayerMovement3D>();
            if (player != null)
            {
                player.TriggerWin();
            }

            if (winTextObject != null)
            {
                winTextObject.SetActive(true);
                var text = winTextObject.GetComponent<TMP_Text>();
                if (text != null) text.text = winMessage;
            }
        }
    }
}