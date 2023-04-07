using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager manager;

    private enum MatchState
    {
        NONE,
        WAITING_FOR_PLAYERS,
        IN_PROGRESS,
        MATCH_ENDED
    }

    private enum VictoryState
    {
        NONE,
        DRAW,
        RED_TEAM_WIN,
        BLUE_TEAM_WIN
    }

    // Syncvar for the time..?
    private Clock redClock, blueClock;
    private MatchState currentMatchState = MatchState.NONE;
    private VictoryState currentVictoryState = VictoryState.NONE;


    private void Awake()
    {
        // Singleton
        if (manager == null)
        {
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameObject[] clocks = GameObject.FindGameObjectsWithTag("Clock");
        foreach(GameObject clock in clocks)
        {
            if(clock.GetComponent<Clock>().teamIdentifier == 0)
            {
                redClock = clock.GetComponent<Clock>();
            }
            else if (clock.GetComponent<Clock>().teamIdentifier == 1)
            {
                blueClock = clock.GetComponent<Clock>();
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(currentMatchState == MatchState.MATCH_ENDED)
        {
            // If match has ended, stop going to update
            return;
        }

        if(redClock.remainingTime < 0 || blueClock.remainingTime < 0)
        {
            // Match ended
            currentMatchState = MatchState.MATCH_ENDED;

            if(redClock.remainingTime < 0 && blueClock.remainingTime < 0)
            {
                // Draw
                currentVictoryState = VictoryState.DRAW;
            }
            else if (redClock.remainingTime < 0)
            {
                // Blue team won
                currentVictoryState = VictoryState.BLUE_TEAM_WIN;
            }
            else if (blueClock.remainingTime < 0)
            {
                // Red team won
                currentVictoryState = VictoryState.RED_TEAM_WIN;
            }

            // Show scoreboard at the end of match
            DisplayScoreboard();
        }
    }

    private void DisplayScoreboard()
    {

    }
}
