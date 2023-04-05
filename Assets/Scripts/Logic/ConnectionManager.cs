using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Managing.Server;

namespace Logic
{
    public class ConnectionManager
    {
        public event Action<int, PlayerConnection> PlayerJoined;
        public event Action<int> PlayerLeft;
        //ServerManager.OnRemoteConnectionState += HandleRemoteConnectionChanged;

        public void start()
        {
        }

        public void OnPlayerConnected(Data.PlayerProfile profile)
        {
            //PlayerJoined.Invoke();
        }

        public void OnPlayerLeft()
        {
            //PlayerLeft.Invoke();
        }
        /*
        private void HandleRemoteConnectionChanged(NetworkConnection connection, RemoteConnectionStateArgs args)
        {
            Debug.Log($"Connection {connection}: {args.ConnectionState}");

            switch (args.ConnectionState)
            {
                case RemoteConnectionState.Stopped:
                    HandleConnectionStopped(connection);
                    break;
                case RemoteConnectionState.Started:
                    HandleConnectionStarted(connection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void HandleConnectionStarted(NetworkConnection connection)
        {
            // kirjaa yhteys ConnectionManageriin
        }
        private void HandleConnectionStopped(NetworkConnection connection)
        {
            // poista yhteys ConnectionManagerista
        }*/
    }
}
