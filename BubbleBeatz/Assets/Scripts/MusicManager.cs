using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float crossfadeTime = 1f;

    [SerializeField] private AudioClip[] musicPlaylist;
    private int currentTrackIndex = 0;

    private Coroutine crossfadeRoutine;

    public void CrossfadeTo(AudioClip newClip, float fadeDuration = 2f)
    {
        StartCoroutine(CrossfadeCoroutine(newClip, fadeDuration));
    }

    public void PlayNextTrack()
    {
        if (musicPlaylist.Length == 0)
            return;

        currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Length;
        CrossfadeTo(musicPlaylist[currentTrackIndex]);
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        // Fade out current music
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        timer = 0f;

        // Fade in new music
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, startVolume, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume; // Ensure it's exactly back
    }
}