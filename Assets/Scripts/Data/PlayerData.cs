using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.PlayerConnection;

namespace Data
{
    public class PlayerData
    {
        public PlayerConnection connection;
        public PlayerProfile profile;
        public int walkSpeed, runSpeed, jumpForce, maxHealth;
    }
}
