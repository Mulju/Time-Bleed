using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronadePackController : NetworkBehaviour
{
    private float respawnTime = 10f;
    PlayerManager playerManager;
    [HideInInspector] public bool isBig = false;
    public ParticleSystem beamEffect;
    public GameObject triplePack;

    [SerializeField] private Transform rotator, r1, r2, r3;
    private float rotation, rot123;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(base.IsServer)
        {
            // Listen only on server to avoid useless event calls
            //MatchManager.matchManager.OnStartMoveChronadePack += MoveChronadeSpawn;
        }

        // Näytä hieno light beam jos on iso
        if(isBig)
        {
            //beamEffect.Play();
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if(base.IsServer)
        {
            //MatchManager.matchManager.OnStartMoveChronadePack -= MoveChronadeSpawn;
        }
    }

    private void Update()
    {
        rotation += Time.deltaTime * 150f;

        rotator.transform.eulerAngles = new Vector3(0, rotation, 0);

        if(isBig)
        {
            rot123 -= Time.deltaTime * 150f;
            r1.transform.eulerAngles = new Vector3(0, rot123, 0);
            r2.transform.eulerAngles = new Vector3(0, rot123, 0);
            r3.transform.eulerAngles = new Vector3(0, rot123, 0);
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
        
        // Stop the cool light beam
        if(isBig)
        {
            beamEffect.Stop();
            triplePack.SetActive(false);
        }
    }

    IEnumerator RespawnChronadePack()
    {
        yield return new WaitForSeconds(respawnTime);
        MoveChronadeSpawn(true);
        ShowChronadePack();
        this.GetComponent<Collider>().enabled = true;

    }

    [ObserversRpc]
    public void MoveChronadeSpawn(bool smth)
    {
        //transform.position = MatchManager.matchManager.nextChronadeSpawn.position;
    }

    [ObserversRpc]
    void ShowChronadePack()
    {
        // Play cool light beam animation
        if(isBig)
        {
            beamEffect.Play();
            triplePack.SetActive(true);
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}