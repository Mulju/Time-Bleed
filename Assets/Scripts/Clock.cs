using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    private float rotation;
    private int hitChronades = 0;
    [SerializeField] private TextMeshPro secondText, minuteText;

    public float remainingSeconds = 60, remainingMinutes = 9, remainingTime = 600;
    public int teamIdentifier;

    void Update()
    {
        rotation += 6 * Time.deltaTime + 30 * hitChronades * Time.deltaTime;

        // Need to round up or down to display it nicely
        remainingSeconds = 60 - rotation / 6;
        if (remainingSeconds < 0)
        {
            rotation = 0;
            remainingMinutes--;
            remainingSeconds = 60;
        }

        secondText.text = "Remaining seconds: " + Mathf.Floor(remainingSeconds);
        minuteText.text = "Remaining minutes: " + remainingMinutes;

        clockHand.transform.localRotation = Quaternion.Euler(0, rotation, 0);

        remainingTime = remainingSeconds + remainingMinutes * 60;
    }

    public int GetIdentifier()
    {
        return teamIdentifier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ChronoGrenade"))
        {
            hitChronades++;
            StartCoroutine(GrenadeTimer());

            // Destroy the grenade
            Destroy(collision.gameObject);

            // Should play some kind of animation here
        }
    }

    IEnumerator GrenadeTimer()
    {
        float remainingTime = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        hitChronades--;
    }
}
