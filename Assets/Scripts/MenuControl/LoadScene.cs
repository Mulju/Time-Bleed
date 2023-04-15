using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTimeBleed()
    {
        SceneManager.LoadScene("TimeBleedLevel1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
