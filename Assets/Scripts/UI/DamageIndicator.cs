using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;

public class DamageIndicator : NetworkBehaviour
{
    private GameObject _player;
    private GameObject _shooter;


    public void SetDamageIndicator(GameObject player, GameObject shooter)
    {
        _player = player;
        _shooter = shooter;
        StartCoroutine(DestroyDamageIndicator());
    }

    private void Update()
    {
        if (_player == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = _shooter.transform.position - _player.transform.position;
        Vector3 playerForward = _player.transform.forward;

        float angle = Vector3.Angle(direction, playerForward);

        if (Vector3.Cross(direction, playerForward).y < 0)
        {
            angle = -angle;
        }
    }

    IEnumerator DestroyDamageIndicator()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}