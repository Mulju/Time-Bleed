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
        footstep, playerHitSound, fragGrenadeSound, clockTick, oneMinute, fiveMinutes, chronadeSpawnMoved, underAttack;
    private List<int> usedSongs = new List<int>();
    private float globalPitch = 1;
    private float globalVolume = 0.1f;

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
        float volume = globalVolume;
        PlaySound(menuMove, volume, globalPitch);
    }

    public void PlaySelectSound()
    {
        float volume = globalVolume;
        PlaySound(menuSelect, volume, globalPitch);
    }

    public void PlayShootSound()
    {
        float volume = globalVolume * 2;
        PlaySound(gunShoot, volume, globalPitch);
    }

    public void PlayWallHit()
    {
        float volume = globalVolume;
        PlaySound(bulletHitWall, volume, globalPitch);
    }

    public void PlayBubbleHit()
    {
        float volume = globalVolume;
        PlaySound(bulletHitBubble, volume, globalPitch);
    }

    public void PlayTimeBindExplosion()
    {
        float volume = globalVolume;
        PlaySound(timeBindExplosion, volume, globalPitch);
    }

    public void PlayChronadeSound()
    {
        float volume = globalVolume;
        PlaySound(chronadeSound, volume, globalPitch);
    }

    public void PlayFootstep()
    {
        float volume = globalVolume;
        PlaySound(footstep, volume, globalPitch);
    }

    public void PlayPlayerHit()
    {
        float volume = globalVolume * 6;
        PlaySound(playerHitSound, volume, globalPitch);
    }

    public void PlayFragExplosion()
    {
        float volume = globalVolume;
        PlaySound(fragGrenadeSound, volume, globalPitch);
    }

    public void PlayClockTick()
    {
        float volume = globalVolume;
        PlaySound(clockTick, volume, globalPitch);
    }

    public void PlayBaseIsUnderAttack()
    {
        float volume = 1f;
        PlaySound(underAttack, volume, globalPitch);
    }

    public void PlayOneMinute()
    {
        float volume = 1f;
        PlaySound(oneMinute, volume, globalPitch);
    }

    public void PlayFiveMinutes()
    {
        float volume = 1f;
        PlaySound(fiveMinutes, volume, globalPitch);
    }

    public void PlayChronadeSpawnMove()
    {
        float volume = 1f;
        PlaySound(chronadeSpawnMoved, volume, globalPitch);
    }
}
