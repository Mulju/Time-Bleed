using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class KillFeed : NetworkBehaviour
{
    [SerializeField] private GameObject _entryTemplate;
    [SerializeField] private Transform _feedContainer;

    private Color redColor = new Color(1, 0, 0.007f, 1);
    private Color greenColor = new Color(0, 1, 0.043f, 1);

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
    private void CreateKillFeed(string killerName, string victimName, int killerTeamTag)
    {
        Transform entryTransform = Instantiate(_entryTemplate, _feedContainer).transform;

        entryTransform.Find("KillerName").GetComponent<TMPro.TextMeshProUGUI>().text = killerName;
        entryTransform.Find("VictimName").GetComponent<TMPro.TextMeshProUGUI>().text = victimName;
        if(killerTeamTag == 0)
        {
            entryTransform.Find("KillerName").GetComponent<TMPro.TextMeshProUGUI>().color = redColor;
            entryTransform.Find("VictimName").GetComponent<TMPro.TextMeshProUGUI>().color = greenColor;
        }
        else
        {
            entryTransform.Find("KillerName").GetComponent<TMPro.TextMeshProUGUI>().color = greenColor;
            entryTransform.Find("VictimName").GetComponent<TMPro.TextMeshProUGUI>().color = redColor;
        }

        Destroy(entryTransform.gameObject, 5);
    }
}