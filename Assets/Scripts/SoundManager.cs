using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    private AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip beginGrab;
    public AudioClip endGrab;
    public AudioClip increaseScale;
    public AudioClip decreaseScale;
    public AudioClip highlightObject;
    public AudioClip clickButton;
    public AudioClip clickKey;
    public AudioClip confirmOrigin;
    public AudioClip resetOrigin;
    public AudioClip selection;
    public AudioClip deselection;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    public void PlaySound(AudioClip newClip)
    {
        //audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
