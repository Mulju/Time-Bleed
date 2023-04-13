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

    private void Update()
    {
        if (exploded)
            return;


        if (timer >= 2f)
        {
            StartCoroutine(SpawnTimeSphere());

            this.gameObject.GetComponent<Collider>().enabled = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.gameObject, timeSphereTime + 2f);
            exploded = true;
        }

        timer += Time.deltaTime;
    }

    IEnumerator SpawnTimeSphere()
    {
        GameObject timeBindSphereInstance = Instantiate(timeBindSphere, this.gameObject.transform.position, Quaternion.identity);
        timeBindSphereInstance.transform.localScale = new Vector3(5f, 5f, 5f);
        Destroy(timeBindSphereInstance, timeSphereTime + 2f);

        yield return new WaitForSeconds(5);

        Debug.Log("moi");
        timeBindSphereInstance.GetComponent<TimeSphere>().ReduceCircumference();
    }
}
