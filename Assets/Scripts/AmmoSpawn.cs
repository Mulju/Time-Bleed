using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawn : MonoBehaviour
{
    public bool isSlowed;
    public bool isInsideTerrain;

    private void FixedUpdate()
    {
        isSlowed = false;
        isInsideTerrain = false;
    }


    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.CompareTag("TimeSphere") && (other.transform.parent == null || other.gameObject.transform.parent.gameObject != gameObject.transform.parent.parent.parent.gameObject))
        {
            isSlowed = true;
        }
        else if (other.gameObject.layer == 6)
        {
            isInsideTerrain = true;
        }
    }
}
