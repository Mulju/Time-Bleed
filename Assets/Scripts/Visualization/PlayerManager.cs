using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    public Dictionary<int, Data.Player> players = new Dictionary<int, Data.Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private MenuControl menuControl;

    public TextMeshProUGUI healthTMP, ammoTMP;

    private void Awake()
    {
        instance = this;
    }

    public void DamagePlayer(int playerID, int damage, int shooterID)
    {
        if (!base.IsServer)
            return;

        players[playerID].health -= damage;
        UpdateHealthUI(players[playerID].connection, players[playerID].playerObject, players[playerID].health);

        if (players[playerID].health <= 0)
        {
            PlayerKilled(playerID, shooterID);
        }
    }

    void PlayerKilled(int playerID, int attackerID)
    {
        print("Player " + playerID.ToString() + " was killed by " + attackerID.ToString());
        players[playerID].deaths++;
        players[playerID].health = 100;
        players[attackerID].kills++;

        RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, spawnPoints.Count));
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn)
    {
        player.transform.position = spawnPoints[spawn].position;
    }

    public void RestoreHealth(GameObject player)
    {
        int playerID = player.GetInstanceID();

        if (players[playerID].health < 100)
        {
            players[playerID].health += 50;

            if (players[playerID].health > 100)
            {
                players[playerID].health = 100;

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