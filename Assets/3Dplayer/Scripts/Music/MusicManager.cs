using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public AudioSource currentMusic;   // The song playing now
    public AudioSource nextMusic;      // The next song to fade into
    public float fadeDuration = 2f;    // How long the fade takes (seconds)

    private bool isFading = false;

    public void CrossfadeTo(AudioSource newTrack)
    {
        if (!isFading && newTrack != null && newTrack != currentMusic)
        {
            StartCoroutine(FadeMusic(newTrack));
        }
    }

    private IEnumerator FadeMusic(AudioSource newTrack)
    {
        isFading = true;

        float timer = 0f;
        float startVolume = currentMusic != null ? currentMusic.volume : 0f;

        // Start new track at 0 volume
        newTrack.volume = 0f;
        newTrack.Play();

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Fade out current, fade in new
            if (currentMusic != null)
                currentMusic.volume = Mathf.Lerp(startVolume, 0f, t);

            newTrack.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Finalize volumes
        if (currentMusic != null)
        {
            currentMusic.Stop();
            currentMusic.volume = 1f; // Reset in case it's reused later
        }

        currentMusic = newTrack;
        isFading = false;
    }
}