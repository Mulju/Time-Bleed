using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HealthPackController : NetworkBehaviour
{
    [SerializeField] private float respawnTime = 5f;
    PlayerManager playerManager;

    private void OnTriggerEnter(Collider col)
    {
        if (!base.IsServer)
            return;

        if (col.CompareTag("Player"))
        {
            playerManager = PlayerManager.instance;
            playerManager.RestoreHealth(col.gameObject);

            HideHealthPack();
            this.GetComponent<Collider>().enabled = false;
            StartCoroutine(RespawnHealthPack());
        }
    }

    [ObserversRpc]
    void HideHealthPack()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Canvas[] canvases = this.gameObject.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            canvas.GetComponent<Canvas>().enabled = false;
        }
    }

    IEnumerator RespawnHealthPack()
    {
        yield return new WaitForSeconds(respawnTime);
        ShowHealthPack();
        this.GetComponent<Collider>().enabled = true;
    }

    [ObserversRpc]
    void ShowHealthPack()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}