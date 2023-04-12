using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HealthPackController : NetworkBehaviour
{
    [SerializeField] private int healthAmount = 50;
    [SerializeField] private float respawnTime = 5f;
    PlayerManager playerManager;

    private void OnTriggerEnter(Collider col)
    {
        if (!base.IsServer)
            return;

        if (col.CompareTag("Player"))
        {
            playerManager = PlayerManager.instance;
            playerManager.RestoreHealth(col.GetInstanceID());
            gameObject.SetActive(false);
            StartCoroutine(RespawnHealthPack());
        }
    }

    IEnumerator RespawnHealthPack()
    {
        yield return new WaitForSeconds(respawnTime);
        gameObject.SetActive(true);
    }
}