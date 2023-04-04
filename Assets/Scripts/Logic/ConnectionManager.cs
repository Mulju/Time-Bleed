using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace Logic
{
    public class ConnectionManager
    {
        public event Action<int, PlayerConnection> PlayerJoined;
        public event Action<int> PlayerLeft;

        public void OnPlayerConnected(Data.PlayerProfile profile)
        {

        }

        public void OnPlayerLeft()
        {

        }
    }
}
