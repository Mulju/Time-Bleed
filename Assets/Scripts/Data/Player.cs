using System;
using FishNet.Connection;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Player
    {
        public int health = 100;
        public GameObject playerObject;
        public NetworkConnection connection;
    }
}