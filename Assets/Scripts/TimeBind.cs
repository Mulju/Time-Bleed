using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBind : MonoBehaviour
{
    public GameObject timeBindSphere;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 2 && !other.CompareTag("TimeSphere"))
        {
            GameObject timeBindSphereInstance = Instantiate(timeBindSphere, this.gameObject.transform.position, Quaternion.identity);
            timeBindSphereInstance.transform.localScale = new Vector3(5f, 5f, 5f);
            Destroy(timeBindSphereInstance, 10);
            Destroy(this.gameObject); 
        }
    }
}
