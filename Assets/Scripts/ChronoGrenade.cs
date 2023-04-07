using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoGrenade : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("TimeSphere"))
        {
            // Reduce the timesphere when hit with Chronade
            col.GetComponent<TimeSphere>().ReduceCircumference();
        }

        if(col.CompareTag("Clock"))
        {
            // Vähennetään aikaa jos on vihollisen kello
        }
    }
}
