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
            Destroy(gameObject);
            // Tee joku hieno animaatio t‰ss‰
        }

        if(col.CompareTag("Clock"))
        {
            // V‰hennet‰‰n aikaa jos on vihollisen kello
        }
    }
}
