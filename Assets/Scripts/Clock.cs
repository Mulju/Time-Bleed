using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEditor;
using System;

public class Clock : NetworkBehaviour
{
    [SerializeField] private Transform[] clockHandAnchors;

    [SerializeField] private GameObject clockHand;
    [SerializeField] private TextMeshPro secondText, minuteText;
    private int hitChronades = 0;

    [SyncVar] public float secondRotation, minuteRotation;
    [SyncVar] public float remainingSeconds, remainingMinutes, remainingTime;



    public int teamIdentifier;
    private MatchManager mManager;
    private PlayerManager pManager;
    [HideInInspector] public int playersKilled = 0;

    [SerializeField] private GameObject[] bigClockHandles;
    public Action<int> OnChronadeHit;
    private int timeGained = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
            pManager = PlayerManager.instance;
        }
    }

    private void Awake()
    {
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
            pManager = PlayerManager.instance;
        }
    }

    private void Start()
    {
        if (base.IsServer)
        {
            mManager = MatchManager.matchManager;
            pManager = PlayerManager.instance;
        }
    }

    void Update()
    {
        if (base.IsServer)
        {
            if (mManager != null && mManager.currentMatchState == MatchManager.MatchState.IN_PROGRESS)
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
        secondRotation += 6 * Time.deltaTime + 60 * hitChronades * Time.deltaTime + 6 * playersKilled * Time.deltaTime - 6 * timeGained * Time.deltaTime;
        minuteRotation += (6 * Time.deltaTime + 60 * hitChronades * Time.deltaTime + 6 * playersKilled * Time.deltaTime - 6 * timeGained * Time.deltaTime) / 15;

        // Need to round up or down to display it nicely
        remainingSeconds = 60 - secondRotation / 6;
        if (remainingSeconds < 0 && remainingMinutes > 0)
        {
            secondRotation = 0;
            remainingMinutes--;
            remainingSeconds = 60;
        }

        if (remainingSeconds < 0 && remainingMinutes == 0)
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
        clockHandAnchors[0].transform.localRotation = Quaternion.Euler(0, 0, secondRotation);
        clockHandAnchors[1].transform.localRotation = Quaternion.Euler(0, 0, minuteRotation);

        bigClockHandles[0].transform.localRotation = Quaternion.Euler(0, 0, secondRotation);
        bigClockHandles[1].transform.localRotation = Quaternion.Euler(0, 0, minuteRotation);
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
            if (OnChronadeHit != null)
            {
                OnChronadeHit.Invoke(teamIdentifier);
            }

            if (base.IsServer)
            {
                pManager.OnChronadeHit(collision.gameObject.GetComponent<ChronoGrenade>().ownerObject.GetInstanceID());
            }

            // Destroy the grenade
            Destroy(collision.gameObject);
        }
    }

    public void OwnTeamPlayerKilled()
    {
        playersKilled++;
        StartCoroutine(ClockTimer(false));
    }

    public void GainTime()
    {
        timeGained++;
        StartCoroutine(GainTimer());
    }

    IEnumerator GainTimer()
    {
        float remainingTime = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        timeGained--;
    }

    IEnumerator ClockTimer(bool isChronade)
    {
        float remainingTime = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        if (isChronade)
        {
            hitChronades--;
        }
        else
        {
            playersKilled--;
        }

        if(base.IsServer)
        {
            mManager.TeamTimeDiffChanged();
        }
    }
}