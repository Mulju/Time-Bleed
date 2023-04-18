using System.Collections.Generic;
using UnityEngine;

public class DmgIndicatorSystem : MonoBehaviour
{
    [SerializeField] private GameObject _damageIndicatorPrefab;
    private List<GameObject> _damageIndicators = new List<GameObject>();

    public void AddDamageIndicator(GameObject player, Vector3 bulletDirection)
    {
        GameObject damageIndicator = Instantiate(_damageIndicatorPrefab, transform);
        DamageIndicator damageIndicatorScript = damageIndicator.GetComponent<DamageIndicator>();
        damageIndicatorScript.SetDamageIndicator(player, bulletDirection);
        _damageIndicators.Add(damageIndicator);
    }
}