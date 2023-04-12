using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuControl : MonoBehaviour
{
    private Resolution[] resolutions;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;

    private string currentScene;

    [SerializeField] private GameObject pauseMenu;

    void Start()
    {
        resolutions = Screen.resolutions;

        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            // Vain main menussa on resoluution vaihto mahdollisuus
            resolutionsDropdown.ClearOptions();
            List<string> options = new List<string>();

            int resolutionIndex = 0;
            int i = 0;
            foreach (Resolution resolution in resolutions)
            {
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);

                // Alla oleva if jotta valitaan sama resoluutio mikä käyttäjällä
                // on tällä hetkellä koneessa valittuna
                if (resolution.width == Screen.width &&
                    resolution.height == Screen.height)
                {
                    resolutionIndex = i;
                }
                i++;
            }

            resolutionsDropdown.AddOptions(options);
            resolutionsDropdown.value = resolutionIndex;
            resolutionsDropdown.RefreshShownValue();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OpenCloseMenu()
    {
        if(currentScene == "TimeBleed")
        {
            if(pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
        }
    }
}