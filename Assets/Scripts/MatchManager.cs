using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
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
    [SerializeField] private GameObject[] chronadePacks;
    //[HideInInspector] public Transform nextChronadeSpawn;
    [HideInInspector] public Action<bool> OnStartMoveChronadePack;
    [HideInInspector] public Action<int> OnClockTimeChange;

    [SerializeField] private GameObject middleTimeSphere;

    private bool fiveHasPlayedRed = false, oneHasPlayedRed = false, fiveHasPlayedGreen = false, oneHasPlayedGreen = false;
    private float oldKillsRatio, oldRedKills;

    [SerializeField] private PlayerUIControl uiControl;

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
                redClock.OnChronadeHit += TeamBaseUnderAttack;
            }
            else if (clock.GetComponent<Clock>().teamIdentifier == 1)
            {
                greenClock = clock.GetComponent<Clock>();
                greenClock.OnChronadeHit += TeamBaseUnderAttack;
            }
        }
    }

    private void OnDisable()
    {
        redClock.OnChronadeHit -= TeamBaseUnderAttack;
        greenClock.OnChronadeHit -= TeamBaseUnderAttack;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        chronadePacks[1].GetComponent<ChronadePackController>().isBig = true;

        if (base.IsServer)
        {
            playerManager = PlayerManager.instance;
            playerManager.OnPlayerKilled += ChangeBigChronadeSpawnServer;
            redClock.OnChronadeHit += TeamBaseUnderAttack;
            greenClock.OnChronadeHit += TeamBaseUnderAttack;

            // Number 2 as that is not a team tag for any team
            ChangeBigChronadeSpawnServer(true, 2);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if(base.IsServer)
        {
            playerManager.OnPlayerKilled -= ChangeBigChronadeSpawnServer;
            redClock.OnChronadeHit -= TeamBaseUnderAttack;
            greenClock.OnChronadeHit -= TeamBaseUnderAttack;
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
            // 2 because it's not a team tag
            ChangeBigChronadeSpawnServer(true, 2);
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



        // Play time sounds
        if(redClock.remainingTime <= 300 && !fiveHasPlayedRed)
        {
            fiveHasPlayedRed = true;
            // Play five sound for red
            OnClockTimeChange.Invoke(0);
        }
        if (greenClock.remainingTime <= 300 && !fiveHasPlayedGreen)
        {
            fiveHasPlayedGreen = true;
            // Play five sound for green
            OnClockTimeChange.Invoke(1);
        }
        if (redClock.remainingTime <= 60 && !oneHasPlayedRed)
        {
            oneHasPlayedRed = true;
            // Play one sound for red
            OnClockTimeChange.Invoke(2);
        }
        if (greenClock.remainingTime <= 60 && !oneHasPlayedGreen)
        {
            oneHasPlayedGreen = true;
            // Play one sound for green
            OnClockTimeChange.Invoke(3);
        }



        // Change match state to ended if time runs out on a clock
        if (redClock.remainingTime <= 0 || greenClock.remainingTime <= 0)
        {
            // Match ended
            currentMatchState = MatchState.MATCH_ENDED;

            if(redClock.remainingTime <= 0 && greenClock.remainingTime <= 0)
            {
                // Draw
                currentVictoryState = VictoryState.DRAW;
            }
            else if (redClock.remainingTime <= 0)
            {
                // Blue team won
                currentVictoryState = VictoryState.BLUE_TEAM_WIN;
            }
            else if (greenClock.remainingTime <= 0)
            {
                // Red team won
                currentVictoryState = VictoryState.RED_TEAM_WIN;
            }

            // Show scoreboard at the end of match
            int waitTime = 20;
            DisplayScoreboard(waitTime);
        }
    }

    public void TeamBaseUnderAttack(int teamID)
    {
        uiControl.UpdateTimeChronadeHit(teamID);
        if(teamID == 0)
        {
            playerManager.PlayAttenborough(4);
        }
        else
        {
            playerManager.PlayAttenborough(5);
        }

        //TeamTimeDiffChanged();
    }

    public bool IsBaseServer()
    {
        return base.IsServer;
    }

    public void ChangeBigChronadeSpawnServer(bool isAtStart, int killerTeamTag)
    {
        oldKillsRatio = playerManager.redKills + playerManager.greenKills;
        oldRedKills = playerManager.redKills;

        playerManager.TotalKills();
        float redKills = playerManager.redKills, greenKills = playerManager.greenKills, totalKills = redKills + greenKills;

        if(totalKills == 0)
        {
            // This is to block strange chronade slider visuals at the start of the match
            redKills = 1;
            totalKills = 2;
        }
        
        if (oldKillsRatio != 0)
        {
            Debug.Log("Oldkills: " + oldKillsRatio + "\nOldRedKills: " + oldRedKills);
            // Check if the Chronade spawn is going to move this frame
            if(redKills / totalKills < 0.4f && !(oldRedKills / oldKillsRatio < 0.4f))
            {
                playerManager.AllClientsPlayChronadeSpawnChange();
            }
            else if((redKills / totalKills > 0.4f && redKills / totalKills < 0.6f) && 
                !(oldRedKills / oldKillsRatio > 0.4f && oldRedKills / oldKillsRatio < 0.6f))
            {
                playerManager.AllClientsPlayChronadeSpawnChange();
            }
            else if(redKills / totalKills > 0.6f && !(oldRedKills / oldKillsRatio > 0.6f))
            {
                playerManager.AllClientsPlayChronadeSpawnChange();
            }
        }

        ChangeBigChronadeSpawn(redKills, totalKills);

        //TeamTimeDiffChanged();
    }

    [ObserversRpc]
    private void ChangeBigChronadeSpawn(float redKills, float totalKills)
    {
        if (redKills / totalKills < 0.4f)
        {
            // Big Chronade spawn on green base's side
            foreach(GameObject chronade in chronadePacks)
            {
                chronade.GetComponent<ChronadePackController>().isBig = false;
                chronade.GetComponent<ChronadePackController>().triplePack.SetActive(false);
                chronade.GetComponent<MeshRenderer>().enabled = true;
                if (chronade.GetComponent<ChronadePackController>().beamEffect.isPlaying)
                {
                    chronade.GetComponent<ChronadePackController>().beamEffect.Stop();
                }
            }

            chronadePacks[2].GetComponent<ChronadePackController>().isBig = true;
            chronadePacks[2].GetComponent<ChronadePackController>().beamEffect.Play();
            chronadePacks[2].GetComponent<ChronadePackController>().triplePack.SetActive(true);
            chronadePacks[2].GetComponent<MeshRenderer>().enabled = false;
        }
        else if (redKills / totalKills > 0.4f && redKills / totalKills < 0.6f)
        {
            // Big Chronade spawn on middle
            foreach (GameObject chronade in chronadePacks)
            {
                chronade.GetComponent<ChronadePackController>().isBig = false;
                chronade.GetComponent<ChronadePackController>().triplePack.SetActive(false);
                chronade.GetComponent<MeshRenderer>().enabled = true;
                if (chronade.GetComponent<ChronadePackController>().beamEffect.isPlaying)
                {
                    chronade.GetComponent<ChronadePackController>().beamEffect.Stop();
                }
            }

            chronadePacks[1].GetComponent<ChronadePackController>().isBig = true;
            chronadePacks[1].GetComponent<ChronadePackController>().beamEffect.Play();
            chronadePacks[1].GetComponent<ChronadePackController>().triplePack.SetActive(true);
            chronadePacks[1].GetComponent<MeshRenderer>().enabled = false;
        }
        else if (redKills / totalKills > 0.6f)
        {
            // Big Chronade spawn on red base's side
            foreach (GameObject chronade in chronadePacks)
            {
                chronade.GetComponent<ChronadePackController>().isBig = false;
                chronade.GetComponent<ChronadePackController>().triplePack.SetActive(false);
                chronade.GetComponent<MeshRenderer>().enabled = true;
                if (chronade.GetComponent<ChronadePackController>().beamEffect.isPlaying)
                {
                    chronade.GetComponent<ChronadePackController>().beamEffect.Stop();
                }
            }

            chronadePacks[0].GetComponent<ChronadePackController>().isBig = true;
            chronadePacks[0].GetComponent<ChronadePackController>().beamEffect.Play();
            chronadePacks[0].GetComponent<ChronadePackController>().triplePack.SetActive(true);
            chronadePacks[0].GetComponent<MeshRenderer>().enabled = false;
        }
        menuControl.UpdateChronadeSlider(redKills / totalKills);
    }

    public void ForceStart()
    {
        currentMatchState = MatchState.STARTING;
    }

    [ObserversRpc]
    private void DisplayScoreboard(int waitTime)
    {
        menuControl.OpenEndMatchScoreboard(currentVictoryState);
        playerManager.scoreboardTimer.gameObject.SetActive(true);
        StartCoroutine(MatchEndTimer(waitTime));

        // Use currentVicoryState to display correct winning team

        // Somekind of end of match music here, possibly different for winning and losing team..?
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

    [ObserversRpc]
    public void TeamTimeDiffChanged()
    {
        int losingTeam;

        if (redClock.remainingTime + 60f < greenClock.remainingTime)
        {
            losingTeam = 0;
        }
        else if (redClock.remainingTime > greenClock.remainingTime + 60f)
        {
            losingTeam = 1;
        }
        else
        {
            // neutral
            losingTeam = 2;
        }

        middleTimeSphere.GetComponent<TimeSphere>().SetTeamTag(losingTeam);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.TryGetComponent<PlayerEntity>(out PlayerEntity script);
            script.UpdateTimeResourceSpendingMultiplier(losingTeam);
        }
        
    }
}