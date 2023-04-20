using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager matchManager;
    private PlayerManager playerManager;

    public enum MatchState
    {
        NONE,
        WAITING_FOR_PLAYERS,
        STARTING,
        IN_PROGRESS,
        MATCH_ENDED
    }

    public enum VictoryState
    {
        NONE,
        DRAW,
        RED_TEAM_WIN,
        BLUE_TEAM_WIN
    }

    // Syncvar for the time..?
    [HideInInspector] public Clock redClock, greenClock;
    [HideInInspector] [SyncVar] public MatchState currentMatchState = MatchState.NONE;
    [HideInInspector] [SyncVar] public VictoryState currentVictoryState = VictoryState.NONE;
    [SerializeField] private MenuControl menuControl;
    [SerializeField] private Transform[] chronadeSpawns;
    [SerializeField] private GameObject chronadePack;


    private void Awake()
    {
        // Singleton
        if (matchManager == null)
        {
            matchManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerManager = PlayerManager.instance;
        currentMatchState = MatchState.WAITING_FOR_PLAYERS;

        GameObject[] clocks = GameObject.FindGameObjectsWithTag("Clock");
        foreach(GameObject clock in clocks)
        {
            if(clock.GetComponent<Clock>().teamIdentifier == 0)
            {
                redClock = clock.GetComponent<Clock>();
            }
            else if (clock.GetComponent<Clock>().teamIdentifier == 1)
            {
                greenClock = clock.GetComponent<Clock>();
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsServer)
        {
            //gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!base.IsServer)
        {
            // Don't run Update if not the server
            return;
        }

        // Still waiting for players
        if(currentMatchState == MatchState.WAITING_FOR_PLAYERS && playerManager.numberOfPlayers == 6)
        {
            // Waiting for players but we have 6 players. Change state to starting the match
            currentMatchState = MatchState.STARTING;
        }

        // Match is starting
        if(currentMatchState == MatchState.STARTING)
        {
            // Run only when the match starts and swap to IN_PROGRESS
            playerManager.StartingMatchServer();
            currentMatchState = MatchState.IN_PROGRESS;
        }

        // Match is in progress
        if(currentMatchState == MatchState.IN_PROGRESS)
        {
            // Check the amount of kills each team has and change the chronade spawn accordingly
            // Change only the spawn location, not the objects location itself
            float redKills = 0, greenKills = 0, totalKills = 0;
            if(redKills / totalKills < 0.4f)
            {

            }

            GameObject spawnedChronade = Instantiate(chronadePack);

            // Different music for different states of the game.
        }

        // Match has ended
        if (currentMatchState == MatchState.MATCH_ENDED)
        {
            return;
        }





        // Change match state to ended if time runs out on a clock
        if(redClock.remainingTime < 0 || greenClock.remainingTime < 0)
        {
            // Match ended
            currentMatchState = MatchState.MATCH_ENDED;

            if(redClock.remainingTime < 0 && greenClock.remainingTime < 0)
            {
                // Draw
                currentVictoryState = VictoryState.DRAW;
            }
            else if (redClock.remainingTime < 0)
            {
                // Blue team won
                currentVictoryState = VictoryState.BLUE_TEAM_WIN;
            }
            else if (greenClock.remainingTime < 0)
            {
                // Red team won
                currentVictoryState = VictoryState.RED_TEAM_WIN;
            }

            // Show scoreboard at the end of match
            DisplayScoreboard();
        }
    }

    public void ForceStart()
    {
        currentMatchState = MatchState.STARTING;
    }

    private void DisplayScoreboard()
    {
        // Use currentVicoryState to display correct winning team

        // Somekind of end of match music here, possibly different for winning and losing team..?
    }
}
