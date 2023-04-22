using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEditor;

public class Clock : NetworkBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private TextMeshPro secondText, minuteText;
    private int hitChronades = 0;

    [SyncVar] public float rotation;
    [SyncVar] public float remainingSeconds = 60, remainingMinutes = 1, remainingTime = 120;
    
    public int teamIdentifier;
    private MatchManager mManager;
    [HideInInspector] public int playersKilled = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
        }
    }

    private void Awake()
    {
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
        }
    }

    private void Start()
    {
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
        }
    }

    void Update()
    {
        if(base.IsServer)
        {
            if(mManager != null && mManager.currentMatchState == MatchManager.MatchState.IN_PROGRESS)
            {
                UpdateClockServer();
            }
        }
    }

    public void UpdateClockServer()
    {
        // Turn clock handle
        // First one turns 6 degrees every second, second one turns 60 degrees (equivalent to 10 seconds on a clock) when hit by a chronade
        // and the last one turn and extra 6 degrees if a player died.
        rotation += 6 * Time.deltaTime + 60 * hitChronades * Time.deltaTime + 6 * playersKilled * Time.deltaTime;

        // Need to round up or down to display it nicely
        remainingSeconds = 60 - rotation / 6;
        if (remainingSeconds < 0 && remainingMinutes > 0)
        {
            rotation = 0;
            remainingMinutes--;
            remainingSeconds = 60;
        }

        if(remainingMinutes < 0)
        {
            remainingSeconds = 0;
            remainingMinutes = 0;
        }

        remainingTime = remainingSeconds + remainingMinutes * 60;
        UpdateClock();
    }

    [ObserversRpc]
    public void UpdateClock()
    {
        secondText.text = "Remaining seconds: " + (remainingSeconds == 0 ? 0 : Mathf.Floor(remainingSeconds));
        minuteText.text = "Remaining minutes: " + remainingMinutes;

        clockHand.transform.localRotation = Quaternion.Euler(0, rotation, 0);
    }

    public int GetIdentifier()
    {
        return teamIdentifier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ChronoGrenade") &&
            collision.gameObject.GetComponent<ChronoGrenade>().ownerObject.GetComponent<PlayerEntity>().ownTeamTag != teamIdentifier)
        {
            hitChronades++;
            StartCoroutine(ClockTimer(true));

            // Destroy the grenade
            Destroy(collision.gameObject);

            // Should play some kind of animation here
        }
    }

    public void OwnTeamPlayerKilled()
    {
        playersKilled++;
        StartCoroutine(ClockTimer(false));
    }

    IEnumerator ClockTimer(bool isChronade)
    {
        float remainingTime = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        if(isChronade)
        {
            hitChronades--;
        }
        else
        {
            playersKilled--;
        }
    }
}