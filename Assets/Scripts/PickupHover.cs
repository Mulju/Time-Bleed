using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHover : MonoBehaviour
{
    public Transform positionA;
    public Transform positionB;
    public float speed = 1f;
    public AnimationCurve curve;

    private float startTime;
    private float journeyLength;

    public int startMultiplier;

    void Start()
    {
        startTime = Time.time * startMultiplier;
        journeyLength = Vector3.Distance(positionA.position, positionB.position);
    }

    void Update()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;
        float pingPong = Mathf.PingPong(fractionOfJourney * 2f, 1f);
        float easedPingPong = curve.Evaluate(pingPong);
        transform.position = Vector3.Lerp(positionA.position, positionB.position, easedPingPong);
    }
}