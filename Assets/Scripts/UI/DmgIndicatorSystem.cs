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

    public void AddDamageIndicator(GameObject player, GameObject shooter)
    {
        GameObject damageIndicator = Instantiate(_damageIndicatorPrefab, transform);
        DamageIndicator damageIndicatorScript = damageIndicator.GetComponent<DamageIndicator>();
        damageIndicatorScript.SetDamageIndicator(player, shooter);
        _damageIndicators.Add(damageIndicatorScript);
    }
}