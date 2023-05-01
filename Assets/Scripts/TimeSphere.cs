using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimeSphere : MonoBehaviour
{
    [HideInInspector]
    public Vector3 originalScale, currentScale;
    public int expansionMultiplier = 5;


    [HideInInspector] public bool isTimeBind;
    [HideInInspector] public bool isTimeField;
    [HideInInspector] public float timeSpeed;
    [HideInInspector] public int teamTag;

    private void Awake()
    {
        timeSpeed = 0.2f;

        isTimeBind = false;
        isTimeField = false;

        originalScale = transform.localScale;
        currentScale = transform.localScale;
    }

    private void Start()
    {
        // set layer and change color based on teamtag 
        if (isTimeBind)
        {
            gameObject.layer = 10 + teamTag;

            if(teamTag == 1)
            {
                gameObject.GetComponent<Renderer>().material.color = new Color(102/225, 255/255, 185/255, gameObject.GetComponent<Renderer>().material.color.a);
            }
        }
    }

    public void ChangeAlpha(float sliderValue)
    {
        // The alpha of the sphere depends on the movement speed of the player
        gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 15 / (sliderValue * 255));
    }

    public void ReduceCircumference()
    {
        StopAllCoroutines();
        StartCoroutine(GetSmaller());
    }

    public void IncreaseCircumference(float speed)
    {
        StopAllCoroutines();
        StartCoroutine(GetBigger(speed));
    }

    IEnumerator GetSmaller()
    {
        while(currentScale.x > 0.0001)
        {
            currentScale = new Vector3(currentScale.x - Time.deltaTime * expansionMultiplier, 
                                       currentScale.y - Time.deltaTime * expansionMultiplier,
                                       currentScale.z - Time.deltaTime * expansionMultiplier);
            transform.localScale = currentScale;
            yield return null;
        }

        yield return new WaitForSeconds(10);

        if (!isTimeBind && !isTimeField)
        {
            StartCoroutine(GetBigger(1f));
        }
        else if (!isTimeField)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator GetBigger(float speed)
    {
        while (currentScale.x < originalScale.x)
        {
            currentScale = new Vector3(currentScale.x + Time.deltaTime * expansionMultiplier * speed,
                                       currentScale.y + Time.deltaTime * expansionMultiplier * speed,
                                       currentScale.z + Time.deltaTime * expansionMultiplier * speed);
            transform.localScale = currentScale;
            yield return null;
        }
    }
}
