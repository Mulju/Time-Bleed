using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBind : MonoBehaviour
{
    public GameObject timeBindSphere;
    public bool exploded;

    private float timeSphereTime;
    private float timer;

    private void Start()
    {
        exploded = false;
        timeSphereTime = 10f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded)
            return;

        StartCoroutine(SpawnTimeSphere());

        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        exploded = true;
    }

    private void Update()
    {
        return;

        if (exploded)
            return;


        if (timer >= 2f)
        {
            StartCoroutine(SpawnTimeSphere());

            this.gameObject.GetComponent<Collider>().enabled = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            exploded = true;
        }

        timer += Time.deltaTime;
    }

    IEnumerator SpawnTimeSphere()
    {
        GameObject timeBindSphereInstance = Instantiate(timeBindSphere, this.gameObject.transform.position, Quaternion.identity);
        timeBindSphereInstance.transform.localScale = new Vector3(7f, 7f, 7f);
        timeBindSphereInstance.GetComponent<TimeSphere>().isTimeBind = true;
        timeBindSphereInstance.GetComponent<TimeSphere>().currentScale = timeBindSphereInstance.transform.localScale;

        yield return new WaitForSeconds(timeSphereTime);

        timeBindSphereInstance.GetComponent<TimeSphere>().ReduceCircumference();
    }
}
