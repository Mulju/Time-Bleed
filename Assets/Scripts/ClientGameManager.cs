using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameManager : MonoBehaviour
{
    public string playerName;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
