using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    Vector3 originalScale, currentScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        currentScale = transform.localScale;
    }

    public void ReduceCircumference()
    {
        StartCoroutine(GetSmaller());
    }

    IEnumerator GetSmaller()
    {
        while(currentScale.x > 0.2)
        {
            currentScale = new Vector3(currentScale.x - Time.deltaTime, 
                                       currentScale.y - Time.deltaTime,
                                       currentScale.z - Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(5);
        StartCoroutine(GetBigger());
    }

    IEnumerator GetBigger()
    {
        while (currentScale.x < originalScale.x)
        {
            currentScale = new Vector3(currentScale.x + Time.deltaTime,
                                       currentScale.y + Time.deltaTime,
                                       currentScale.z + Time.deltaTime);
            yield return null;
        }
    }
}
