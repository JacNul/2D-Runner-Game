using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicTrigger3D : MonoBehaviour
{
    public MusicManagerClipBased musicManager;
    public AudioClip clipToPlay;
    public bool singleUse = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (musicManager == null)
        {
            Debug.LogWarning("MusicTrigger3D: MusicManager not assigned.");
            return;
        }

        Debug.Log("MusicTrigger3D: Player entered trigger, switching music.");
        musicManager.CrossfadeToClip(clipToPlay);

        if (singleUse) gameObject.SetActive(false);
    }
}
