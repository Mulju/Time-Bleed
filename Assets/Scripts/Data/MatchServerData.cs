using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MatchServerData
    {
        public enum MatchState
        {
            NONE,
            WAITING_FOR_PLAYERS,
            IN_PROGRESS,
            RESTARTING,
            FINISHED
        }

        public MatchState state = MatchState.NONE;
        public Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
    }
}
