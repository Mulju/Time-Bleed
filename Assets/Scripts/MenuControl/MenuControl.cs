using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using FishNet.Transporting.Tugboat;
using UnityEngine.UI;
using FishNet.Managing.Server;
using FishNet.Managing.Client;
using FishNet.Managing;

public class MenuControl : MonoBehaviour
{
    private Resolution[] resolutions;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;

    private string currentScene;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject scoreboard;
    private EventSystem eventSystem;

    [SerializeField] private GameObject networkManager;
    [SerializeField] private Slider chronadeSlider;
    [SerializeField] private NetworkManager netManager;

    void Start()
    {
        resolutions = Screen.resolutions;

        currentScene = SceneManager.GetActiveScene().name;
        eventSystem = EventSystem.current;

        // Vain main menussa on resoluution vaihto mahdollisuus
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();

        int resolutionIndex = 0;
        int i = 0;
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            // Alla oleva if jotta valitaan sama resoluutio mik� k�ytt�j�ll�
            // on t�ll� hetkell� koneessa valittuna
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

    public void SelectActive(GameObject selected)
    {
        eventSystem.SetSelectedGameObject(selected);
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
        if (currentScene == "TimeBleed")
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
        }
    }

    public void OpenCloseScoreboard()
    {
        if (currentScene == "TimeBleed")
        {
            if (scoreboard.activeSelf)
            {
                scoreboard.SetActive(false);
            }
            else
            {
                scoreboard.SetActive(true);
            }
        }

    }

    public void OpenEndMatchScoreboard(MatchManager.VictoryState victoriousTeam)
    {
        // Aseta scoreboardille voittava tiimi näkyviin

        // Ehkä myös timeri joka palauttaa pelaajat main menuun?
        
        scoreboard.SetActive(true);
    }

    public void UpdateChronadeSlider(float fillAmount)
    {
        chronadeSlider.value = fillAmount;
    }

    public void UpdateTugboat(string clientAddress)
    {
        networkManager.GetComponent<Tugboat>().SetClientAddress(clientAddress);
    }

    public void DisconnectFromServer()
    {
        netManager.ClientManager.StopConnection();
    }
}