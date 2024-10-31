using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    public static EnemyAudioController instance1 { get; private set; }
    public AudioSource audioSource;

    void Start()
    {
        instance1 = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRoarSound(AudioClip roarSound)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(roarSound);
    }

    public void PlayFireSound(AudioClip fireSound)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(fireSound);
    }

    public void PlayAttackSound(AudioClip attackSound)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(attackSound);
    }

    public void PlayArrowSound(AudioClip arrowSound)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(arrowSound);
    }

}
