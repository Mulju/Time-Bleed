using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    private float rotation;

    private int hitChronades = 0;
    
    void Update()
    {
        rotation += 6 * Time.deltaTime + 30 * hitChronades * Time.deltaTime;
        clockHand.transform.localRotation = Quaternion.Euler(0, rotation, 0);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("ChronoGrenade"))
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

        while(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        hitChronades--;
    }
}
