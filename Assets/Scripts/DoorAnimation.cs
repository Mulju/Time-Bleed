using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : NetworkBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private PlayerManager playerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerManager = PlayerManager.instance;
    }

    private void OnEnable()
    {
        playerManager.OnStartingMatch += OpenCloseDoor;
    }

    private void OnDisable()
    {
        playerManager.OnStartingMatch -= OpenCloseDoor;
    }

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
