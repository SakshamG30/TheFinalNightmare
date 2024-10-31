using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public static PlayerAudioController instance { get; private set; }
    private AudioSource audioSource;

    void Start()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound(AudioClip jumpSound)
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayJumpLanding(AudioClip landingSound)
    {
        audioSource.PlayOneShot(landingSound);
    }

    public void PlayGravityInvertSound(AudioClip gravitySound)
    {
        audioSource.PlayOneShot(gravitySound);
    }

    public void PlayAttackSound(AudioClip attackSound)
    {
        audioSource.PlayOneShot(attackSound);
    }
}
