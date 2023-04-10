using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameManager : MonoBehaviour
{
    public Data.Player playerData = new Data.Player();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
