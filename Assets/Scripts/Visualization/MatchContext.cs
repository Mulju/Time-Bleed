using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using UnityEngine.Networking.PlayerConnection;
using Logic;

public class MatchContext : NetworkBehaviour
{
    public GameObject player;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner) GetComponent<MatchContext>().enabled = false;

        PlayerConnection connection = new PlayerConnection();
        int id = connection.GetInstanceID();
        SpawnPlayerEntity(id);
        //ServerManager.OnRemoteConnectionState += HandleRemoteConnectionChanged;
    }

    [ServerRpc]
    public void SpawnPlayerEntity(int id)
    {
        // Tarttee vielä positionin ja rotaation
        GameObject spawnedPlayer = Instantiate(player);
        spawnedPlayer.GetComponent<PlayerEntity>().connectionID = id;
        ServerManager.Spawn(spawnedPlayer);
    }
}
