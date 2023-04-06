using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoGrenade : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("TimeSphere"))
        {
            // Jotain tapahtuu kun osutaan aikakuplaan
        }

        if(col.CompareTag("Clock"))
        {
            // Vähennetään aikaa jos on vihollisen kello
        }
    }
}
