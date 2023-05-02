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
using FishNet.Object;

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
    public Image[] chronadeImages;

    public bool settingsOpen;
    public bool menuOpen;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsButton;
    [HideInInspector] public float mouseSensitivity;

    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private TextMeshProUGUI redUIKills, redUITime, greenUIKills, greenUITime;
    [SerializeField] private TextMeshProUGUI redKPlus, redTPlus, greenKPlus, greenTPlus;
    private int newRedKills = 0, newRedTime = 0, newGreenKills = 0, newGreenTime = 0;


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

        mouseSensitivity = 1;
    }

    private void OnEnable()
    {
        if(SceneManager.GetActiveScene().name == "TimeBleed")
        {
            playerManager.OnPlayerKilled += UpdateUIKills;
        }
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "TimeBleed")
        {
            playerManager.OnPlayerKilled -= UpdateUIKills;
        }
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

    public void SetMouseSensitivity(float sens)
    {
        mouseSensitivity = sens;
    }

    public void SetOpenMenu()
    {
        menuOpen = false;
    }

    public void OpenCloseMenu()
    {
        if (currentScene == "TimeBleed")
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                menuOpen = false;
            }
            else
            {
                pauseMenu.SetActive(true);
                menuOpen = true;
            }
        }
    }

    public void OpenSettings()
    {
        settingsOpen = true;
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        settingsOpen = false;
        SelectActive(settingsButton);
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

    [ObserversRpc]
    public void UpdateUIKills(bool irrelevant, int teamTag)
    {
        playerManager.TotalKills();

        if(teamTag == 0)
        {
            redUIKills.text = "" + playerManager.redKills;
            newRedKills++;
            redKPlus.gameObject.SetActive(true);
            redKPlus.text = "+" + newRedKills;

            // Refresh the timer on the plus
            StopCoroutine(ShowKillPlus(true));
            StartCoroutine(ShowKillPlus(true));
        }
        else
        {
            greenUIKills.text = "" + playerManager.greenKills;
            newGreenKills++;
            greenKPlus.gameObject.SetActive(true);
            greenKPlus.text = "+" + newGreenKills;

            StopCoroutine(ShowKillPlus(false));
            StartCoroutine(ShowKillPlus(false));
        }
    }

    public void UpdateUITimes()
    {

    }

    IEnumerator ShowKillPlus(bool isRed)
    {
        yield return new WaitForSeconds(3);

        if(isRed)
        {
            redKPlus.gameObject.SetActive(false);
            newRedKills = 0;
        }
        else
        {
            greenKPlus.gameObject.SetActive(false);
            newGreenKills = 0;
        }
    }
}