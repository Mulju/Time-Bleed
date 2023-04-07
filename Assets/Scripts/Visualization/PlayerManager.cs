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

    public void DamagePlayer(int playerID, int damage)
    {
        if (!base.IsServer)
            return;

        players[playerID].health -= damage;

        if (players[playerID].health <= 0)
        {
            PlayerKilled(playerID);
        }
    }

    void PlayerKilled(int playerID)
    {
        Data.Player player = players[playerID];
        player.health = 100;

        RespawnPlayer(player.connection, player.playerObject, Random.Range(0, spawnPoints.Count));
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection connnection, GameObject player, int spawn)
    {
        Debug.Log("Respawning player");
        player.transform.position = spawnPoints[spawn].position;
    }
}