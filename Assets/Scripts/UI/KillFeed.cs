using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class KillFeed : NetworkBehaviour
{
    [SerializeField] private GameObject _entryTemplate;
    [SerializeField] private Transform _feedContainer;

    PlayerManager _playerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _playerManager = PlayerManager.instance;
        _playerManager.OnKillFeedUpdate += CreateKillFeed;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _playerManager.OnKillFeedUpdate -= CreateKillFeed;
    }

    [ObserversRpc]
    private void CreateKillFeed(string killerName, string victimName)
    {
        Transform entryTransform = Instantiate(_entryTemplate, _feedContainer).transform;

        entryTransform.Find("KillerName").GetComponent<TMPro.TextMeshProUGUI>().text = killerName;
        entryTransform.Find("VictimName").GetComponent<TMPro.TextMeshProUGUI>().text = victimName;

        Destroy(entryTransform.gameObject, 5);
    }
}