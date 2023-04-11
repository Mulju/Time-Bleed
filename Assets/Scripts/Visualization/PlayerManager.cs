using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;
    private void Awake()
    {
        instance = this;
    }

    public Dictionary<int, Data.Player> players = new Dictionary<int, Data.Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    public void DamagePlayer(int playerID, int damage, int shooterID)
    {
        if (!base.IsServer)
            return;

        players[playerID].health -= 50;

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
}