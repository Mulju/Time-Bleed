using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum MatchState
    {
        NONE,
        WAITING_FOR_PLAYERS,
        IN_PROGRESS,
        MATCH_ENDED
    }
    public static GameManager manager;
    public float redTeamTime, blueTeamTime;

    public GameObject redClock, blueClock;

    // Syncvar?
    private MatchState currentState = MatchState.NONE;


    private void Awake()
    {
        // Singleton
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(redTeamTime < 0 && blueTeamTime < 0)
        {
            // Draw
        }
        else if (redTeamTime < 0)
        {
            // Blue team won
        }
        else if (blueTeamTime < 0)
        {
            // Red team won
        }
    }
}
