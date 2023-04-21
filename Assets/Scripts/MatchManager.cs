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
    [HideInInspector] public Transform nextChronadeSpawn;
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

        nextChronadeSpawn = chronadeSpawns[1];

        if (base.IsServer)
        {
            playerManager = PlayerManager.instance;
            playerManager.OnPlayerKilled += MoveChronadeSpawnServer;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if(base.IsServer)
        {
            playerManager.OnPlayerKilled -= MoveChronadeSpawnServer;
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
            //GameObject spawnedChronade = Instantiate(chronadePack);

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
            StartOwnEndTimer(20);
        }
    }

    public bool IsBaseServer()
    {
        return base.IsServer;
    }

    private void MoveChronadeSpawnServer(bool smth)
    {
        playerManager.TotalKills();
        float redKills = playerManager.redKills, greenKills = playerManager.greenKills, totalKills = redKills + greenKills;
        MoveChronadeSpawn(redKills, totalKills);
    }

    [ObserversRpc]
    private void MoveChronadeSpawn(float redKills, float totalKills)
    {
        if (redKills / totalKills < 0.4f)
        {
            // Chronade spawn on green base's side

            nextChronadeSpawn = chronadeSpawns[2];
        }
        else if (redKills / totalKills > 0.4f && redKills / totalKills < 0.6f)
        {
            // Chronade spawn on middle

            nextChronadeSpawn = chronadeSpawns[1];
        }
        else if (redKills / totalKills > 0.6f)
        {
            // Chronade spawn on red base's side

            nextChronadeSpawn = chronadeSpawns[0];
        }
        menuControl.UpdateChronadeSlider(redKills / totalKills);
    }

    public void ForceStart()
    {
        currentMatchState = MatchState.STARTING;
    }

    [ObserversRpc]
    private void DisplayScoreboard()
    {
        menuControl.OpenEndMatchScoreboard(currentVictoryState);

        // Use currentVicoryState to display correct winning team

        // Somekind of end of match music here, possibly different for winning and losing team..?
    }

    [ObserversRpc]
    public void StartOwnEndTimer(int waitTime)
    {
        playerManager.scoreboardTimer.gameObject.SetActive(true);
        StartCoroutine(MatchEndTimer(waitTime));
    }

    IEnumerator MatchEndTimer(int waitTime)
    {
        while(waitTime >= 0)
        {
            playerManager.scoreboardTimer.text = "Match ends in\n" + waitTime;
            yield return new WaitForSeconds(1);
            waitTime--;
        }
        playerManager.scoreboardTimer.gameObject.SetActive(false);

        if(base.IsServer)
        {
            playerManager.CloseServer();
        }
    }
}