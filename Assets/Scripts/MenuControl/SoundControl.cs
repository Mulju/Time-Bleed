using FishNet.Managing.Timing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using FishNet.Managing;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private AudioSource[] soundSources;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private PlayerEntity playerEntity;
    [SerializeField] private AudioClip menuMove, menuSelect, gunShoot, bulletHitWall, bulletHitBubble, timeBindExplosion, chronadeSound,
        footstep, playerHitSound, fragGrenadeSound, clockTick;
    private List<int> usedSongs = new List<int>();
    private float globalPitch = 1;

    private void Update()
    {
        //// Don't run this code in Main Menu
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (playerEntity.IsOwnerOfPlayer() && !musicSource.isPlaying)
        {
            // If you're the owner of this object and the music source stopped playing audio, loop through the not used BG musics and play one
            int number;
            do
            {
                number = Random.Range(0, 7);
            } while (usedSongs.Contains(number));

            usedSongs.Add(number);
            PlayNextBGMusic(musicClips[number]);
        }
    }

    public void PlaySound(AudioClip soundEffect, float volume, float pitch)
    {
        foreach (AudioSource source in soundSources)
        {
            // Loop through soundSource array. If there is a source that's not playing, use it
            if (!source.isPlaying)
            {
                source.clip = soundEffect;
                source.volume = volume;
                source.pitch = pitch;
                source.Play();
                break;
            }
        }
    }

    public void PlayNextBGMusic(AudioClip musicToPlay)
    {
        musicSource.clip = musicToPlay;
        musicSource.Play();
    }

    public void PlayMoveSound()
    {
        float volume = 0.2f;
        PlaySound(menuMove, volume, globalPitch);
    }

    public void PlaySelectSound()
    {
        float volume = 0.2f;
        PlaySound(menuSelect, volume, globalPitch);
    }

    public void PlayShootSound()
    {
        float volume = 0.4f;
        PlaySound(gunShoot, volume, globalPitch);
    }

    public void PlayWallHit()
    {
        float volume = 0.2f;
        PlaySound(bulletHitWall, volume, globalPitch);
    }

    public void PlayBubbleHit()
    {
        float volume = 0.2f;
        PlaySound(bulletHitBubble, volume, globalPitch);
    }

    public void PlayTimeBindExplosion()
    {
        float volume = 0.2f;
        PlaySound(timeBindExplosion, volume, globalPitch);
    }

    public void PlayChronadeSound()
    {
        float volume = 0.2f;
        PlaySound(chronadeSound, volume, globalPitch);
    }

    public void PlayFootstep()
    {
        float volume = 0.2f;
        PlaySound(footstep, volume, globalPitch);
    }

    public void PlayPlayerHit()
    {
        float volume = 0.6f;
        PlaySound(playerHitSound, volume, globalPitch);
    }

    public void PlayFragExplosion()
    {
        float volume = 0.2f;
        PlaySound(fragGrenadeSound, volume, globalPitch);
    }

    public void PlayClockTick()
    {
        float volume = 0.2f;
        PlaySound(clockTick, volume, globalPitch);
    }
}
