using FishNet.Managing.Timing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private AudioSource[] soundSources;
    [SerializeField] private AudioClip menuMove, menuSelect, gunShoot;


    public void PlaySound(AudioClip soundEffect, float volume)
    {
        foreach (AudioSource source in soundSources)
        {
            // Loop through soundSource array. If there is a source that's not playing, use it
            if (!source.isPlaying)
            {
                source.clip = soundEffect;
                source.volume = volume;
                source.Play();
                break;
            }
        }
    }

    public void PlayMoveSound()
    {
        float volume = 0.2f;
        PlaySound(menuMove, volume);
    }

    public void PlaySelectSound()
    {
        float volume = 0.2f;
        PlaySound(menuSelect, volume);
    }

    public void PlayShootSound()
    {
        float volume = 0.4f;
        PlaySound(gunShoot, volume);
    }
}
