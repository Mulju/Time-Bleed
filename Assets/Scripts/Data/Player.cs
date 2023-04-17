using System;
using FishNet.Connection;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Player
    {
        public string name;
        public int health = 100;
        public int kills = 0;
        public int deaths = 0;
        public GameObject playerObject;
        public NetworkConnection connection;
        public int teamTag;
    }
}