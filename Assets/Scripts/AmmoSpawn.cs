using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawn : MonoBehaviour
{
    public bool isSlowed;

    private void FixedUpdate()
    {
        isSlowed = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere") && other.gameObject.transform.parent.gameObject != gameObject.transform.parent.parent.parent.gameObject)
        {
            isSlowed = true;
        }
    }
}
