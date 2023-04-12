using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBind : MonoBehaviour
{
    public GameObject timeBindSphere;

    private float timer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 2 && !collision.gameObject.CompareTag("TimeSphere"))
        {
            //GameObject timeBindSphereInstance = Instantiate(timeBindSphere, this.gameObject.transform.position, Quaternion.identity);
            //timeBindSphereInstance.transform.localScale = new Vector3(5f, 5f, 5f);
            //Destroy(timeBindSphereInstance, 10);
            //Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (timer >= 2f)
        {
            GameObject timeBindSphereInstance = Instantiate(timeBindSphere, this.gameObject.transform.position, Quaternion.identity);
            timeBindSphereInstance.transform.localScale = new Vector3(5f, 5f, 5f);
            Destroy(timeBindSphereInstance, 10);
            Destroy(this.gameObject);
        }

        timer += Time.deltaTime;
    }
}
