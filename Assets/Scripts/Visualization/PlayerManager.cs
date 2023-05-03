using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using FishNet;
using FishNet.Transporting;
using FishNet.Managing.Server;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditor;
using LiteNetLib;
using FishNet.Managing;
using UnityEngine.UI;
using UnityEngine.Playables;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    private Dictionary<int, Data.Player> players = new Dictionary<int, Data.Player>();
    [SerializeField] private List<Transform> redSpawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> greenSpawnPoints = new List<Transform>();

    [SerializeField] private MenuControl menuControl;
    [SerializeField] private GameObject scoreboard;

    public TextMeshProUGUI healthTMP, ammoTMP;
    private int maxHealth = 100;
    private bool redTeamTurn = true;

    [SerializeField] private TextMeshProUGUI serverNumberOfPlayers;
    [SerializeField] private GameObject startMatchTimer;

    [HideInInspector] public int numberOfPlayers;

    [SerializeField] private Animator doorAnimator;
    public event Action<bool> OnStartingMatch;
    public event Action<bool, int> OnPlayerKilled;
    public event Action<string, string, int> OnKillFeedUpdate;
    public event Action<ServerConnectionStateArgs> OnServerConnectionState;

    [HideInInspector]
    public int redKills, greenKills;

    private bool playerKilledThisFrame = false;

    [SerializeField] private NetworkManager netManager;
    [SerializeField] private LoadScene sceneLoader;
    public TextMeshProUGUI scoreboardTimer;
    [SerializeField] private GameObject waitingForPlayersText;

    [SerializeField] private PlayerUIControl uiControl;

    private void Awake()
    {
        instance = this;
        numberOfPlayers = 0;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            ServerManager.OnRemoteConnectionState += NmrPlayersChanged;
            MatchManager.matchManager.OnClockTimeChange += PlayAttenborough;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (base.IsServer)
        {
            ServerManager.OnRemoteConnectionState -= NmrPlayersChanged;
            MatchManager.matchManager.OnClockTimeChange -= PlayAttenborough;
        }
    }

    private void Update()
    {
        if (base.IsServer)
        {
            playerKilledThisFrame = false;

            foreach (KeyValuePair<int, Data.Player> player in players)
            {
                if (player.Value.playerObject == null)
                {
                    continue;
                }

                if (player.Value.playerObject.transform.position.y < -10)
                {
                    PlayerKilled(player.Key, player.Key);
                }
            }
        }
    }

    public void PlayAttenborough(int i)
    {
        // Plays the Attenborough sounds for remaining time

        //List<PlayerEntity> redPlayers = new List<PlayerEntity>();
        //List<PlayerEntity> greenPlayers = new List<PlayerEntity>();
        foreach (KeyValuePair<int, Data.Player> player in players)
        {
            if (player.Value.playerObject.GetComponent<PlayerEntity>().ownTeamTag == 0)
            {
                // Is a red player
                if (i == 0)
                {
                    ClientPlayAttenborough(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>(), true);

                    //player.Value.playerObject.GetComponent<PlayerEntity>().soundControl.PlayFiveMinutes();
                }
                else if (i == 2)
                {
                    ClientPlayAttenborough(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>(), false);

                    //player.Value.playerObject.GetComponent<PlayerEntity>().soundControl.PlayOneMinute();
                }
                else if (i == 4)
                {
                    ClientPlayBaseUnderAttack(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>());
                }

                //redPlayers.Add(player.Value.playerObject.GetComponent<PlayerEntity>());
            }
            else
            {
                // Is a green player
                if (i == 1)
                {
                    ClientPlayAttenborough(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>(), true);

                    //player.Value.playerObject.GetComponent<PlayerEntity>().soundControl.PlayFiveMinutes();
                }
                else if (i == 3)
                {
                    ClientPlayAttenborough(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>(), false);

                    //player.Value.playerObject.GetComponent<PlayerEntity>().soundControl.PlayOneMinute();
                }
                else if (i == 5)
                {
                    ClientPlayBaseUnderAttack(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>());
                }

                //greenPlayers.Add(player.Value.playerObject.GetComponent<PlayerEntity>());
            }
        }
        /*
        switch(i)
        {
            case 0:
                // Red team 5 minutes
                foreach(PlayerEntity player in redPlayers)
                {
                    ClientPlayAttenborough(player.);
                }
                break;

            case 1:
                // Green team 5 minutes
                foreach (PlayerEntity player in greenPlayers)
                {
                    player.soundControl.PlayFiveMinutes();
                }
                break;

            case 2:
                // Red team 1 minute
                foreach (PlayerEntity player in redPlayers)
                {
                    player.soundControl.PlayOneMinute();
                }
                break;

            case 3:
                // Green team 1 minute
                foreach (PlayerEntity player in greenPlayers)
                {
                    player.soundControl.PlayOneMinute();
                }
                break;

            default:
                break;
        }*/
    }

    [TargetRpc]
    public void ClientPlayAttenborough(NetworkConnection connection, PlayerEntity player, bool isFiveMinutes)
    {
        if (isFiveMinutes)
        {
            player.soundControl.PlayFiveMinutes();
        }
        else
        {
            player.soundControl.PlayOneMinute();
        }
    }

    [TargetRpc]
    public void ClientPlayBaseUnderAttack(NetworkConnection connection, PlayerEntity player)
    {
        player.soundControl.PlayBaseIsUnderAttack();
    }

    public void AllClientsPlayChronadeSpawnChange()
    {
        foreach (KeyValuePair<int, Data.Player> player in players)
        {
            ClientPlayChronadeSpawnChange(player.Value.connection, player.Value.playerObject.GetComponent<PlayerEntity>());
        }
    }

    [TargetRpc]
    public void ClientPlayChronadeSpawnChange(NetworkConnection connection, PlayerEntity player)
    {
        player.soundControl.PlayChronadeSpawnMove();
    }

    public void NmrPlayersChanged(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            // Someone joined, do something? Perhaps kick the connecting player if 6 already on server?
            // Propably call AddPlayer here and remove the call from PlayerEntity
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            // Someone left
            RemovePlayer(connection);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerName(NetworkConnection connection, string name)
    {
        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            if (pair.Value.connection == connection)
            {
                pair.Value.name = name;
            }
        }
    }

    public void AddPlayer(int id, Data.Player player)
    {
        if (redTeamTurn)
        {
            // Is in the red team
            player.teamTag = 0;
        }
        else
        {
            // Is in green team
            player.teamTag = 1;
        }

        redTeamTurn = !redTeamTurn;
        players.Add(id, player);
        numberOfPlayers++;
        serverNumberOfPlayers.text = numberOfPlayers + " / 6\nPlayers";

        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            ChangePlayerTeam(pair.Value);
        }

        UpdateScoreboard();

        // This is to get the chronade pack animation to play for new players. 2 as a parameter as that's not a team tag
        MatchManager.matchManager.ChangeBigChronadeSpawnServer(false, 2);
    }

    public void RemovePlayer(NetworkConnection connection)
    {
        Dictionary<int, Data.Player> playersCopy = new Dictionary<int, Data.Player>(players);

        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            if (pair.Value.connection == connection)
            {
                playersCopy.Remove(pair.Key);
                numberOfPlayers--;
            }
        }

        redTeamTurn = !redTeamTurn;
        players = new Dictionary<int, Data.Player>(playersCopy);
        serverNumberOfPlayers.text = numberOfPlayers + " / 6\nPlayers";
    }

    [ObserversRpc]
    private void ChangePlayerTeam(Data.Player player)
    {
        player.playerObject.GetComponent<PlayerEntity>().ChangeTeam(player.teamTag);
    }

    public void StartingMatchServer()
    {
        // Only called on the server

        int redIndex = 0, greenIndex = 0;
        // Move all players to their own spawns and reset all the kills and deaths
        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            if (pair.Value.teamTag == 0)
            {
                RespawnPlayer(pair.Value.connection, pair.Value.playerObject, redIndex, 0);
                redIndex++;
                if (redIndex == 4)
                {
                    redIndex = 0;
                }
            }
            else
            {
                RespawnPlayer(pair.Value.connection, pair.Value.playerObject, greenIndex, 1);
                greenIndex++;
                if (greenIndex == 4)
                {
                    greenIndex = 0;
                }
            }

            pair.Value.kills = 0;
            pair.Value.deaths = 0;
            pair.Value.stolenTime = 0;
        }

        // Close spawn doors and start a timer to open them
        OnStartingMatch.Invoke(false);
        StartCoroutine(ServerDoorTimer());

        // Display a timer for match start
        StartMatch();

        // Reset the scoreboards
        UpdateScoreboard();

        uiControl.UpdateUIKillsServer(true, 0);
        uiControl.UpdateUIKillsServer(true, 1);

        // Destroy all ammos left in the scene. Can't use a foreach as we're changing the array at run time
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Ammo");
        int length = bullets.Length;
        for (int i = 0; i < length; i++)
        {
            Destroy(bullets[i]);
        }

        // After start timer is finished, change match state to in progress. Might not need this. MatchManager's update does this for now.
        // MatchManager.matchManager.currentMatchState = MatchManager.MatchState.IN_PROGRESS;
    }


    [ObserversRpc]
    public void StartMatch()
    {
        startMatchTimer.SetActive(true);
        waitingForPlayersText.SetActive(false);
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        int timer = 15;
        while (timer >= 0)
        {
            // Display timer on screen here
            startMatchTimer.GetComponent<TextMeshProUGUI>().text = "Match starts in\n" + timer;

            yield return new WaitForSeconds(1);
            timer--;
        }

        startMatchTimer.SetActive(false);
    }

    IEnumerator ServerDoorTimer()
    {
        int timer = 15;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }
        OnStartingMatch.Invoke(true);


        // Reset the clock timers
        MatchManager.matchManager.redClock.secondRotation = 0;
        MatchManager.matchManager.redClock.remainingSeconds = 60;
        MatchManager.matchManager.redClock.remainingMinutes = 14;
        MatchManager.matchManager.redClock.remainingTime = 900;

        MatchManager.matchManager.greenClock.secondRotation = 0;
        MatchManager.matchManager.greenClock.remainingSeconds = 60;
        MatchManager.matchManager.greenClock.remainingMinutes = 14;
        MatchManager.matchManager.greenClock.remainingTime = 900;
    }

    public void DamagePlayer(int playerID, int damage, int shooterID)
    {

        if (!base.IsServer)
            return;

        if (players[playerID].health <= 0)
        {
            // Don't reduce hp if already 0 or below. This will prevent players getting multiple kills
            // within the same frame by using shotgun on a low hp player.
            return;
        }

        players[playerID].health -= damage;

        if (players[playerID].health <= 0)
        {
            // If player dies, update UI with max health
            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, maxHealth);
        }
        else
        {
            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        }

        if (players[playerID].health <= 0)
        {
            PlayerKilled(playerID, shooterID);
        }

        ClientPlayDamageSound(players[playerID].connection, players[playerID].playerObject);
        ClientPlayHitSound(players[shooterID].connection, players[shooterID].playerObject);
    }

    [TargetRpc]
    private void ClientPlayDamageSound(NetworkConnection connection, GameObject player)
    {
        player.GetComponent<PlayerEntity>().soundControl.PlayPlayerDamage();
    }

    [TargetRpc]
    private void ClientPlayHitSound(NetworkConnection connection, GameObject player)
    {
        player.GetComponent<PlayerEntity>().soundControl.PlayPlayerHit();
        StartCoroutine(HitMarker());
    }

    IEnumerator HitMarker()
    {
        GameObject.Find("CombatUI/Crosshair/HitMarker").SetActive(true);
        yield return new WaitForSeconds(0.05f);
        GameObject.Find("CombatUI/Crosshair/HitMarker").SetActive(false);
    }

    void PlayerKilled(int playerID, int attackerID)
    {
        if (playerKilledThisFrame)
        {
            // If PlayerKilled was called this frame, return. This prevents multikills.
            return;
        }
        playerKilledThisFrame = true;

        StartRagdoll(players[playerID].playerObject);

        if (attackerID != playerID)
        {
            players[attackerID].kills++;
            OnPlayerKilled.Invoke(false, players[attackerID].teamTag);
            OnKillFeedUpdate.Invoke(players[attackerID].name, players[playerID].name, players[attackerID].teamTag);

            // Regenerate time resource
            players[attackerID].playerObject.GetComponent<PlayerEntity>().timeResource = 4;
            // Stop possible cooldown on the regeneration on the player that died
            players[playerID].playerObject.GetComponent<PlayerEntity>().resourceOnCooldown = false;
        }
        players[playerID].deaths++;

        if (players[playerID].teamTag == 0)
        {
            // Respawn at red team's base and reduce 1 second from the clock
            RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, redSpawnPoints.Count), players[playerID].teamTag);
            MatchManager.matchManager.redClock.OwnTeamPlayerKilled();
            MatchManager.matchManager.greenClock.GainTime();
            players[attackerID].stolenTime += 1;

            if (MatchManager.matchManager.redClock.remainingTime - MatchManager.matchManager.greenClock.remainingTime >= 60)
            {
                MatchManager.matchManager.redClock.OwnTeamPlayerKilled();
                MatchManager.matchManager.greenClock.GainTime();
                players[attackerID].stolenTime += 1;
            }
        }
        else
        {
            // Respawn at green team's base and reduce 1 second from the clock
            RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, greenSpawnPoints.Count), players[playerID].teamTag);
            MatchManager.matchManager.greenClock.OwnTeamPlayerKilled();
            MatchManager.matchManager.redClock.GainTime();
            players[attackerID].stolenTime += 1;

            if (MatchManager.matchManager.greenClock.remainingTime - MatchManager.matchManager.redClock.remainingTime >= 60)
            {
                // Red player killed a green player, and the red team is over a minute behind. Gain an additional second
                MatchManager.matchManager.greenClock.OwnTeamPlayerKilled();
                MatchManager.matchManager.redClock.GainTime();
                players[attackerID].stolenTime += 1;
            }
        }

        // Debuggausta
        int playerIndex = 1;
        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            Debug.Log("Player " + playerIndex + " Kills: " + pair.Value.kills + ", Deaths: " + pair.Value.deaths);
            playerIndex++;
        }

        UpdateScoreboard();

        StartCoroutine(MaxHealth(playerID));
    }

    IEnumerator MaxHealth(int playerID)
    {
        yield return new WaitForSeconds(5f);

        if (players[playerID] != null)
        {
            players[playerID].health = maxHealth;
            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        }
    }

    IEnumerator DeathCam(GameObject player, int spawn, int teamTag)
    {
        SkinnedMeshRenderer[] meshes = player.GetComponent<PlayerEntity>().playerMesh.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.renderingLayerMask = 1;
        }

        player.GetComponent<PlayerEntity>().ArmorInvisibleServer();
        player.GetComponent<PlayerEntity>().BodyVisibleServer();
        player.GetComponent<PlayerEntity>().RigidbodyNotKinematicServer();

        player.GetComponent<PlayerEntity>().currentWeaponAnimationsPrefab.SetActive(true);

        player.GetComponent<PlayerEntity>().isAlive = false;

        yield return new WaitForSeconds(5f);

        player.GetComponent<PlayerEntity>().isAlive = true;

        PlayerReset(player, spawn, teamTag);
    }

    void PlayerReset(GameObject player, int spawn, int teamTag)
    {
        if (teamTag == 0)
        {
            // Respawn at red team's base
            player.transform.position = redSpawnPoints[spawn].position;
        }
        else
        {
            // Respawn at green team's base
            player.transform.position = greenSpawnPoints[spawn].position;
        }

        player.GetComponent<PlayerEntity>().amountOfChronades = 1;
        for (int i = 1; i < 3; i++)
        {
            // Disable the latter 2 chronade images as you only have 1 when you respawn
            menuControl.chronadeImages[i].GetComponentInChildren<Text>().enabled = false;
            menuControl.chronadeImages[i].enabled = false;
        }

        player.GetComponent<PlayerEntity>().RespawnServer();

        Camera.main.transform.localPosition = new Vector3(0, 1f, 0);
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn, int teamTag)
    {
        StartCoroutine(DeathCam(player, spawn, teamTag));
    }

    [ObserversRpc]
    void StartRagdoll(GameObject player)
    {
        PlayerEntity playerEntity = player.GetComponent<PlayerEntity>();
        playerEntity.currentWeaponPrefab.SetActive(false);
        playerEntity.timeField.SetActive(false);
        playerEntity.nameDisplay.SetActive(false);
        playerEntity.playerAnimator.enabled = false;
        playerEntity.canMove = false;
        playerEntity.characterController.enabled = false;
    }

    public void RestoreHealth(GameObject player)
    {
        int playerID = player.GetInstanceID();

        player.GetComponent<PlayerEntity>().soundControl.PlayHealthPickup();

        if (players[playerID].health < maxHealth)
        {
            players[playerID].health += 40;

            if (players[playerID].health > maxHealth)
            {
                players[playerID].health = maxHealth;

            }

            UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
        }
    }

    [ObserversRpc]
    public void AddChronades(GameObject player, bool isBig)
    {
        // Give the player 3 Chronades if it was big. If it was small, give the player 1 Chronade but only up to 3.
        if (isBig)
        {
            player.GetComponent<PlayerEntity>().amountOfChronades = 3;
        }
        else
        {
            if (player.GetComponent<PlayerEntity>().amountOfChronades < 3)
            {
                player.GetComponent<PlayerEntity>().amountOfChronades++;
            }
            else
            {
                player.GetComponent<PlayerEntity>().amountOfChronades = 3;
            }
        }
        player.GetComponent<PlayerEntity>().soundControl.PlayChronadePickup();
    }

    [TargetRpc]
    public void UpdateHealthUI(NetworkConnection conn, GameObject player, int health)
    {
        player.GetComponent<PlayerEntity>().healthTMP.text = "HP - " + health;
    }

    public void ChangeCursorLock()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TotalKills()
    {
        redKills = 0;
        greenKills = 0;
        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            if (pair.Value.teamTag == 0)
            {
                redKills += pair.Value.kills;
            }
            else
            {
                greenKills += pair.Value.kills;
            }
        }
    }

    public void UpdateScoreboard()
    {
        players = players.OrderBy(x => x.Value.stolenTime).ToDictionary(x => x.Key, x => x.Value);
        players = players.Reverse().ToDictionary(x => x.Key, x => x.Value);

        TotalKills();
        LoopScores(players, redKills, greenKills);
    }

    [ObserversRpc]
    void LoopScores(Dictionary<int, Data.Player> players, int redKills, int greenKills)
    {
        scoreboard.GetComponent<ScoreTable>().DestroyScores();
        int redRank = 1;
        int greenRank = 1;
        foreach (KeyValuePair<int, Data.Player> pair in players)
        {
            if (pair.Value.teamTag == 0)
            {
                scoreboard.GetComponent<ScoreTable>().UpdateScore(pair.Value.name, pair.Value.kills, pair.Value.deaths, pair.Value.stolenTime, pair.Value.teamTag, redRank);
                redRank++;
            }
            else
            {
                scoreboard.GetComponent<ScoreTable>().UpdateScore(pair.Value.name, pair.Value.kills, pair.Value.deaths, pair.Value.stolenTime, pair.Value.teamTag, greenRank);
                greenRank++;
            }
        }
        scoreboard.GetComponent<ScoreTable>().UpdateBoard(redKills, greenKills);
    }

    public void CloseServer()
    {
        if (base.IsServer)
        {
            ClientOnServerClose();
            /*
            foreach(KeyValuePair<int, Data.Player> pair in players)
            {
                //netManager.ServerManager.Kick(pair.Value.connection, KickReason.Unset);
            }*/

            StartCoroutine(KickClients());
        }
        else
        {
            netManager.ClientManager.StopConnection();
            sceneLoader.LoadMainMenu();
        }
    }

    IEnumerator KickClients()
    {
        yield return new WaitForSeconds(1);
        netManager.ServerManager.StopConnection(true);
        sceneLoader.LoadMainMenu();
    }

    [ObserversRpc]
    public void ClientOnServerClose()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!base.IsServer)
        {
            sceneLoader.LoadMainMenu();
        }
    }

    public void OnChronadeHit(int id)
    {
        players[id].stolenTime += 10;
        UpdateScoreboard();
    }
}