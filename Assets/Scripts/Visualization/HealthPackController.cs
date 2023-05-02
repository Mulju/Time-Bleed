using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HealthPackController : NetworkBehaviour
{
    [SerializeField] private float respawnTime = 5f;
    PlayerManager playerManager;

    [SerializeField] private GameObject rotator;
    private float rotation;

    private void Start()
    {
        rotation = 0;
    }

    private void Update()
    {
        rotation += Time.deltaTime * 150f;

        rotator.transform.eulerAngles = new Vector3(0, rotation, 0);
    }

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
        //this.gameObject.GetComponent<MeshRenderer>().enabled = false;

        //Canvas[] canvases = this.gameObject.GetComponentsInChildren<Canvas>();

        //foreach (Canvas canvas in canvases)
        //{
        //    canvas.GetComponent<Canvas>().enabled = false;
        //}

        MeshRenderer[] renderers = transform.parent.parent.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
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
        //this.gameObject.GetComponent<MeshRenderer>().enabled = true;

        //Canvas[] canvases = this.gameObject.GetComponentsInChildren<Canvas>();

        //foreach (Canvas canvas in canvases)
        //{
        //    canvas.GetComponent<Canvas>().enabled = true;
        //}

        MeshRenderer[] renderers = transform.parent.parent.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}