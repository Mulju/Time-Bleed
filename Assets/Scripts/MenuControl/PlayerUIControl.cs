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
using System;
using Unity.VisualScripting;

public class PlayerUIControl : NetworkBehaviour
{
    
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private TextMeshProUGUI redUIKills, redUITime, greenUIKills, greenUITime;
    [SerializeField] private TextMeshProUGUI redKPlus, redTPlus, greenKPlus, greenTPlus;
    private int newRedKills = 0, newRedTime = 0, newGreenKills = 0, newGreenTime = 0;

    private Coroutine redKillTimer, greenKillTimer, redTimeTimer, greenTimeTimer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerManager.OnPlayerKilled += UpdateUIKillsServer;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        playerManager.OnPlayerKilled -= UpdateUIKillsServer;
    }

    private void UpdateUIKillsServer(bool ïrrelevant, int teamTag)
    {
        playerManager.TotalKills();

        if (teamTag == 0)
        {
            UpdateUIKills(teamTag, playerManager.redKills);
        }
        else
        {
            UpdateUIKills(teamTag, playerManager.greenKills);
        }
    }

    [ObserversRpc]
    public void UpdateUIKills(int teamTag, int amountOfKills)
    {
        if(teamTag == 0)
        {
            redUIKills.text = "" + amountOfKills;
            newRedKills++;
            redKPlus.gameObject.SetActive(true);
            redKPlus.text = "+" + newRedKills;

            // Refresh the timer on the plus
            if(redKillTimer != null)
            {
                StopCoroutine(redKillTimer);
            }
            redKillTimer = StartCoroutine(ShowKillPlus(true));
        }
        else
        {
            greenUIKills.text = "" + amountOfKills;
            newGreenKills++;
            greenKPlus.gameObject.SetActive(true);
            greenKPlus.text = "+" + newGreenKills;

            if (greenKillTimer != null)
            {
                StopCoroutine(greenKillTimer);
            }
            greenKillTimer = StartCoroutine(ShowKillPlus(false));
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