using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    private Vector3 originalScale, currentScale;
    private int expansionMultiplier = 5;

    private void Awake()
    {
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
            currentScale = new Vector3(currentScale.x - Time.deltaTime * expansionMultiplier, 
                                       currentScale.y - Time.deltaTime * expansionMultiplier,
                                       currentScale.z - Time.deltaTime * expansionMultiplier);
            transform.localScale = currentScale;
            yield return null;
        }

        yield return new WaitForSeconds(10);
        StartCoroutine(GetBigger());
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
