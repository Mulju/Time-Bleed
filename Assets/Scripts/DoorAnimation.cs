using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class DoorAnimation : NetworkBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private PlayerManager playerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(base.IsServer)
        {
            playerManager = PlayerManager.instance;
            playerManager.OnStartingMatch += OpenCloseDoorServer;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if(base.IsServer)
        {
            playerManager.OnStartingMatch -= OpenCloseDoorServer;
        }
    }

    [ServerRpc]
    public void OpenCloseDoorServer(bool openDoor)
    {
        OpenCloseDoor(openDoor);
    }

    [ObserversRpc]
    public void OpenCloseDoor(bool openDoor)
    {
        if(openDoor)
        {
            doorAnimator.SetBool("DoorIsOpen", true);
        }
        else
        {
            doorAnimator.SetBool("DoorIsOpen", false);
        }
    }
}
