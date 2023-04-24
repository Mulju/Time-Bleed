using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronadePackController : NetworkBehaviour
{
    private float respawnTime = 10f;
    PlayerManager playerManager;
    [HideInInspector] public bool isBig = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(base.IsServer)
        {
            // Listen only on server to avoid useless event calls
            MatchManager.matchManager.OnStartMoveChronadePack += MoveChronadeSpawn;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if(base.IsServer)
        {
            MatchManager.matchManager.OnStartMoveChronadePack -= MoveChronadeSpawn;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!base.IsServer)
            return;

        if (col.CompareTag("Player"))
        {
            playerManager = PlayerManager.instance;
            playerManager.AddChronades(col.gameObject, isBig);

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
        MoveChronadeSpawn(true);
        ShowChronadePack();
        this.GetComponent<Collider>().enabled = true;

        if(isBig)
        {
            // Play cool light beam animation
        }
    }

    [ObserversRpc]
    public void MoveChronadeSpawn(bool smth)
    {
        transform.position = MatchManager.matchManager.nextChronadeSpawn.position;
    }

    [ObserversRpc]
    void ShowChronadePack()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}