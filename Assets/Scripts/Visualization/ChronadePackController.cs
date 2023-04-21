using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronadePackController : NetworkBehaviour
{
    private float respawnTime = 10f;
    PlayerManager playerManager;

    private void OnTriggerEnter(Collider col)
    {
        if (!base.IsServer)
            return;

        if (col.CompareTag("Player"))
        {
            playerManager = PlayerManager.instance;
            playerManager.AddChronades(col.gameObject);

            HideChronadePack();
            this.GetComponent<Collider>().enabled = false;
            StartCoroutine(RespawnChronadePack());
        }
    }

    [ObserversRpc]
    void HideChronadePack()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator RespawnChronadePack()
    {
        yield return new WaitForSeconds(respawnTime);
        MoveChronadeSpawn();
        ShowChronadePack();
        this.GetComponent<Collider>().enabled = true;
    }

    [ObserversRpc]
    public void MoveChronadeSpawn()
    {
        transform.position = MatchManager.matchManager.nextChronadeSpawn.position;
    }

    [ObserversRpc]
    void ShowChronadePack()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}