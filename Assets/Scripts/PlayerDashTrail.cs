using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer[] trails;

    private void Awake()
    {
        Destroy(this.gameObject, 2);
        StartCoroutine(StopTrails());
    }

    IEnumerator StopTrails()
    {
        yield return new WaitForSeconds(0.4f);

        foreach(TrailRenderer trail in trails)
        {
            trail.emitting = false;
        }
    }
}
