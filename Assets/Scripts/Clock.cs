using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Clock : NetworkBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private TextMeshPro secondText, minuteText;
    private int hitChronades = 0;

    [SyncVar] private float rotation;
    [SyncVar] public float remainingSeconds = 60, remainingMinutes = 14, remainingTime = 900;
    
    public int teamIdentifier;

    void Update()
    {
        UpdateClock();
    }

    //[ServerRpc]
    public void UpdateClock()
    {
        // Turn clock handle
        rotation += 6 * Time.deltaTime + 60 * hitChronades * Time.deltaTime;

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