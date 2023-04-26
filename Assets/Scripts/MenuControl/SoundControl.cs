using FishNet.Managing.Timing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private AudioSource[] soundSources;
    [SerializeField] private AudioClip menuMove, menuSelect, gunShoot;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;
    private List<int> usedSongs = new List<int>();

    private void Update()
    {
        // Don't run this code in Main Menu
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if(!musicSource.isPlaying)
        {
            // Music source stopped playing audio, loop through the not used BG musics and play one
            int number;
            do
            {
                number = Random.Range(0, 8);
            } while (usedSongs.Contains(number));
            
            usedSongs.Add(number);
            PlayNextBGMusic(musicClips[number]);
        }
    }

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

    public void PlayNextBGMusic(AudioClip musicToPlay)
    {
        musicSource.clip = musicToPlay;
        musicSource.Play();
    }
}
