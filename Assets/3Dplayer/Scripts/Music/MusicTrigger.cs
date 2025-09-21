using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManagerClipBased : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip startingClip;     // clip to play on start (optional)
    [Header("Fade settings")]
    public float fadeDuration = 2f;
    [Header("Debug")]
    public bool enableDebugHotkey = false; // press debugKey to swap to debugClip
    public KeyCode debugKey = KeyCode.M;
    public AudioClip debugClip;

    private AudioSource srcA;
    private AudioSource srcB;
    private AudioSource currentSource;
    private bool isFading = false;

    void Awake()
    {
        // Create two AudioSources on this GameObject so we can crossfade between them
        srcA = gameObject.AddComponent<AudioSource>();
        srcB = gameObject.AddComponent<AudioSource>();
        ConfigureSource(srcA);
        ConfigureSource(srcB);

        currentSource = srcA;

        if (startingClip != null)
        {
            currentSource.clip = startingClip;
            currentSource.volume = 1f;
            currentSource.Play();
        }
    }

    void ConfigureSource(AudioSource s)
    {
        s.playOnAwake = false;
        s.loop = true;
        s.spatialBlend = 0f; // 2D sound
        s.volume = 1f;
    }

    void Update()
    {
        if (enableDebugHotkey && debugClip != null && Input.GetKeyDown(debugKey))
        {
            CrossfadeToClip(debugClip);
        }
    }

    // Public method to call from triggers: pass AudioClip you want to fade into
    public void CrossfadeToClip(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("MusicManagerClipBased: newClip is null.");
            return;
        }
        if (isFading)
        {
            Debug.Log("MusicManagerClipBased: already fading.");
            return;
        }

        // If the same clip is already playing, do nothing
        if (currentSource.clip == newClip)
        {
            Debug.Log("MusicManagerClipBased: that clip is already playing.");
            return;
        }

        AudioSource nextSource = (currentSource == srcA) ? srcB : srcA;
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();
        StartCoroutine(FadeRoutine(nextSource));
    }

    private IEnumerator FadeRoutine(AudioSource nextSource)
    {
        isFading = true;
        float t = 0f;
        float startVolume = currentSource.volume;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);
            currentSource.volume = Mathf.Lerp(startVolume, 0f, normalized);
            nextSource.volume = Mathf.Lerp(0f, 1f, normalized);
            yield return null;
        }

        // finalize
        currentSource.Stop();
        currentSource.volume = 1f; // reset
        currentSource = nextSource;
        isFading = false;
    }
}