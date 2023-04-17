using System.Collections;
using UnityEngine;
using FishNet.Object;

public class DamageIndicator : NetworkBehaviour
{
    private GameObject _player;
    Vector3 _bulletDirection;


    public void SetDamageIndicator(GameObject player, Vector3 bulletDirection)
    {
        _player = player;
        _bulletDirection = bulletDirection;
        StartCoroutine(DestroyDamageIndicator());
    }

    private void Update()
    {
        if (_player == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 playerForward = _player.transform.forward;

        float angle = Vector3.Angle(_bulletDirection, playerForward);

        if (Vector3.Cross(_bulletDirection, playerForward).y < 0)
        {
            angle = -angle;
        }

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator DestroyDamageIndicator()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}