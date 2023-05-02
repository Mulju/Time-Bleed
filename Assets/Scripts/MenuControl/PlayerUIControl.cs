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

    private void Update()
    {
        float redSeconds = MathF.Floor(MatchManager.matchManager.redClock.remainingSeconds);
        float greenSeconds = MathF.Floor(MatchManager.matchManager.greenClock.remainingSeconds);
        redUITime.text = MatchManager.matchManager.redClock.remainingMinutes + ":" + (redSeconds < 10 ? "0" + redSeconds : redSeconds);
        greenUITime.text = MatchManager.matchManager.greenClock.remainingMinutes + ":" + (greenSeconds < 10 ? "0" + greenSeconds : greenSeconds);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerManager.OnPlayerKilled += UpdateUIKillsServer;
        playerManager.OnPlayerKilled += UpdateUITimesServer;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        playerManager.OnPlayerKilled -= UpdateUIKillsServer;
        playerManager.OnPlayerKilled -= UpdateUITimesServer;
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
            redKillTimer = StartCoroutine(ShowPlus(true, true));
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
            greenKillTimer = StartCoroutine(ShowPlus(false, true));
        }
    }

    public void UpdateTimeChronadeHit(int teamTag)
    {
        UpdateUITimes(teamTag, true);
    }

    public void UpdateUITimesServer(bool irrelevant, int teamTag)
    {
        UpdateUITimes(teamTag, false);
    }

    [ObserversRpc]
    public void UpdateUITimes(int teamTag, bool hitWithChronade)
    {
        if(teamTag == 0)
        {
            if(hitWithChronade)
            {
                newRedTime -= 10;
            }
            else
            {
                newRedTime++;
                if (MatchManager.matchManager.greenClock.remainingTime - MatchManager.matchManager.redClock.remainingTime >= 60)
                {
                    newRedTime++;
                }
            }
            redTPlus.gameObject.SetActive(true);
            // - if the change in time is below 0, + otherwise
            redTPlus.text = ((newRedTime < 0) ? " " : "+") + newRedTime;

            if(redTimeTimer != null)
            {
                StopCoroutine(redTimeTimer);
            }
            redTimeTimer = StartCoroutine(ShowPlus(true, false));
        }
        else
        {
            if(hitWithChronade)
            {
                newGreenTime -= 10;
            }
            else
            {
                newGreenTime++;
                if (MatchManager.matchManager.redClock.remainingTime - MatchManager.matchManager.greenClock.remainingTime >= 60)
                {
                    newGreenTime++;
                }
            }
            greenTPlus.gameObject.SetActive(true);
            greenTPlus.text = ((newGreenTime < 0) ? " " : "+") + newGreenTime;

            if (greenTimeTimer != null)
            {
                StopCoroutine(greenTimeTimer);
            }
            greenTimeTimer = StartCoroutine(ShowPlus(false, false));
        }
    }

    IEnumerator ShowPlus(bool isRed, bool isKill)
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Plus timer ended");
        if(isRed)
        {
            if(isKill)
            {
                redKPlus.gameObject.SetActive(false);
                newRedKills = 0;
            }
            else
            {
                redTPlus.gameObject.SetActive(false);
                newRedTime = 0;
            }
        }
        else
        {
            if(isKill)
            {
                greenKPlus.gameObject.SetActive(false);
                newGreenKills = 0;
            }
            else
            {
                greenTPlus.gameObject.SetActive(false);
                newGreenTime = 0;
            }
        }
    }
}