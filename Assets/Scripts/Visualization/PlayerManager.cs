using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    private Dictionary<int, Data.Player> players = new Dictionary<int, Data.Player>();
    [SerializeField] private List<Transform> redSpawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> greenSpawnPoints = new List<Transform>();

    [SerializeField] private MenuControl menuControl;

    public TextMeshProUGUI healthTMP, ammoTMP;
    private int maxHealth = 100;
    private bool redTeamTurn = true;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!base.IsServer)
            return;

        foreach (KeyValuePair<int, Data.Player> player in players)
        {
            if(player.Value.playerObject == null)
            {
                continue;
            }

            if (player.Value.playerObject.transform.position.y < -10)
            {
                PlayerKilled(player.Key, player.Key);
            }
        }
    }

    public void AddPlayer(int id, Data.Player player)
    {
        if(redTeamTurn)
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

        foreach(KeyValuePair<int, Data.Player> pair in players)
        {
            ChangePlayerTeam(pair.Value);        
        }
    }

    public void RemovePlayer(NetworkConnection connection)
    {
        foreach(KeyValuePair<int, Data.Player> pair in players)
        {
            if(pair.Value.connection == connection)
            {
                players.Remove(pair.Key);
            }
        }
    }

    [ObserversRpc]
    private void ChangePlayerTeam(Data.Player player)
    {
        player.playerObject.GetComponent<PlayerEntity>().ChangeTeam(player.teamTag);
    }

    public void DamagePlayer(int playerID, int damage, int shooterID)
    {

        if (!base.IsServer)
            return;

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
    }

    void PlayerKilled(int playerID, int attackerID)
    {
        if (attackerID != playerID)
        {
            players[attackerID].kills++;
        }
        players[playerID].deaths++;
        players[playerID].health = maxHealth;

        if(players[playerID].teamTag == 0)
        {
            // Respawn at red team's base
            RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, redSpawnPoints.Count), players[playerID].teamTag);
        }
        else
        {
            // Respawn at green team's base
            RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, greenSpawnPoints.Count), players[playerID].teamTag);
        }
        players[playerID].health = maxHealth;

        StartCoroutine(MaxHealth(playerID));
    }

    IEnumerator MaxHealth(int playerID)
    {
        yield return new WaitForSeconds(1f);

        players[playerID].health = maxHealth;
        UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn, int teamTag)
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

        player.GetComponent<PlayerEntity>().RespawnServer();
    }

    public void RestoreHealth(GameObject player)
    {
        int playerID = player.GetInstanceID();

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
}