using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    [HideInInspector]
    public Vector3 originalScale, currentScale;
    private int expansionMultiplier = 5;

    [HideInInspector]
    public bool isTimeBind;

    private void Awake()
    {
        isTimeBind = false;

        originalScale = transform.localScale;
        currentScale = transform.localScale;
    }

    public void ReduceCircumference()
    {
        StopAllCoroutines();
        StartCoroutine(GetSmaller());
    }

    IEnumerator GetSmaller()
    {
        while(currentScale.x > 0.01)
        {
            Debug.Log(currentScale.x + " " + currentScale.y + " " + currentScale.z);
            currentScale = new Vector3(currentScale.x - Time.deltaTime * expansionMultiplier, 
                                       currentScale.y - Time.deltaTime * expansionMultiplier,
                                       currentScale.z - Time.deltaTime * expansionMultiplier);
            transform.localScale = currentScale;
            yield return null;
        }

        yield return new WaitForSeconds(10);

        if (!isTimeBind)
        {
            StartCoroutine(GetBigger());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator GetBigger()
    {
        while (currentScale.x < originalScale.x)
        {
            currentScale = new Vector3(currentScale.x + Time.deltaTime * expansionMultiplier,
                                       currentScale.y + Time.deltaTime * expansionMultiplier,
                                       currentScale.z + Time.deltaTime * expansionMultiplier);
            transform.localScale = currentScale;
            yield return null;
        }
    }
}
