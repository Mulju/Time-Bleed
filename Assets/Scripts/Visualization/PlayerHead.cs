using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerHead : NetworkBehaviour
{
    public GameObject player;

    private float damageMultiplier;

    private void Start()
    {
        damageMultiplier = player.GetComponent<PlayerEntity>().headDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo") && other.TryGetComponent<AmmoController>(out AmmoController ammo) && other.GetComponent<AmmoController>().shooter != player.gameObject)
        {
            if (base.IsServer)
            {
                player.GetComponent<PlayerEntity>().Hit(player.gameObject, ammo.shooter, damageMultiplier);
                player.GetComponent<PlayerEntity>().ShowDamageDirection(player.gameObject, other.GetComponent<AmmoController>().direction);
            }

            Destroy(other.gameObject);
        }
    }
}
