using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using FishNet.Connection;

public class DmgIndicatorSystem : NetworkBehaviour
{
    [SerializeField] private GameObject _damageIndicatorPrefab;
    private List<DamageIndicator> _damageIndicators = new List<DamageIndicator>();

    public void AddDamageIndicator(GameObject player, Vector3 bulletDirection)
    {
        GameObject damageIndicator = Instantiate(_damageIndicatorPrefab, transform);
        DamageIndicator damageIndicatorScript = damageIndicator.GetComponent<DamageIndicator>();
        damageIndicatorScript.SetDamageIndicator(player, bulletDirection);
        _damageIndicators.Add(damageIndicatorScript);
    }
}