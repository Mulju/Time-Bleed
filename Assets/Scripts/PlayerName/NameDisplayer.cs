using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class NameDisplayer : NetworkBehaviour
{
    [SerializeField]
    private TextMeshPro _text;

    private PlayerManager pManager;

    private void Start()
    {
        pManager = PlayerManager.instance;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        pManager = PlayerManager.instance;
        SetName();
        PlayerNameTracker.OnNameChange += PlayerNameTracker_OnNameChange;

    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        PlayerNameTracker.OnNameChange -= PlayerNameTracker_OnNameChange;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        SetName();
    }


    private void PlayerNameTracker_OnNameChange(NetworkConnection arg1, string arg2)
    {
        if (arg1 != base.Owner)
            return;

        SetName();
    }


    /// <summary>
    /// Sets Text to the name for this objects owner.
    /// </summary>
    private void SetName()
    {
        string result = null;
        //Owner does nto exist.
        if (base.Owner.IsValid)
            result = PlayerNameTracker.GetPlayerName(base.Owner);

        if (string.IsNullOrEmpty(result))
            result = "Unset";

        _text.text = result;
        pManager.AddPlayerName(base.Owner, result);
    }
}