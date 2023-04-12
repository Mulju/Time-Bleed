using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HealthPackController : NetworkBehaviour
{
    [SerializeField] private int healthAmount = 50;
    [SerializeField] private float respawnTime = 5f;

    private void OnTriggerEnter(Collider col)
    {
        if (!base.IsServer)
            return;

        if (col.CompareTag("Player"))
        {
            PlayerManager pm = PlayerManager.instance;
            pm.RsetoreHealth(col.GetInstanceID());
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