using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource SFXSource;

    [Header("Audio clips")]
    public AudioClip bellSoundEffect;
    public AudioClip cardSoundEffect;
    public AudioClip pageTurnSFX;
    public AudioClip pauseSFX;
    public AudioClip btnPressSFX;


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
        SFXSource.PlayDelayed(5);
    }

    public void PlaySFX2(AudioClip clip)
    {
        SFXSource.PlayDelayed(5);
        SFXSource.PlayOneShot(clip);
        SFXSource.PlayDelayed(5);
    }
}