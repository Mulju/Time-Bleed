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

        players[playerID].health -= damage;

        if (players[playerID].health <= 0)
        {
            PlayerKilled(playerID, shooterID);
        }
    }

    void PlayerKilled(int playerID, int shooterID)
    {
        Data.Player player = players[playerID];
        Data.Player shooter = players[playerID];

        player.health = 100;
        player.deaths++;

        shooter.kills++;

        Debug.Log("Player " + player.name + ", deaths: " + player.deaths + ", kills: " + player.kills);

        RespawnPlayer(player.connection, player.playerObject, Random.Range(0, spawnPoints.Count));
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection connnection, GameObject player, int spawn)
    {
        player.transform.position = spawnPoints[spawn].position;
    }
}