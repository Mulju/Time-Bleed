using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioVolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] private Slider[] audioSliders;
    [SerializeField] private bool updateSliders;

    private void Awake()
    {
        if(updateSliders && SceneManager.GetActiveScene().name == "TimeBleed")
        {
            float masterVal, musicVal, soundVal;
            audioMixer.GetFloat("masterVol", out masterVal);
            audioMixer.GetFloat("musicVol", out musicVal);
            audioMixer.GetFloat("soundVol", out soundVal);

            audioSliders[0].value = Mathf.Pow(10, masterVal / 20);
            audioSliders[1].value = Mathf.Pow(10, musicVal / 20);
            audioSliders[2].value = Mathf.Pow(10, soundVal / 20);
        }
    }

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
