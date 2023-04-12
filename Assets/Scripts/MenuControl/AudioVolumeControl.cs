using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("masterVol", Mathf.Log10(value) * 20);
    }
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVol", Mathf.Log10(value) * 20);
    }
    public void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("soundVol", Mathf.Log10(value) * 20);
    }
}
