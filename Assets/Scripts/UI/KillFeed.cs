using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class KillFeed : MonoBehaviour
{
    [SerializeField] private GameObject _entryTemplate;
    [SerializeField] private Transform _feedContainer;

    PlayerManager _playerManager;

    private void OnEnable()
    {
        _playerManager = PlayerManager.instance;
        _playerManager.OnKillFeedUpdate += CreateKillFeed;
    }

    private void OnDisable()
    {
        _playerManager.OnKillFeedUpdate -= CreateKillFeed;
    }

    private void CreateKillFeed(string killerName, string victimName)
    {
        Transform entryTransform = Instantiate(_entryTemplate, _feedContainer).transform;

        entryTransform.Find("FeedInstance/KillerName").GetComponent<TMPro.TextMeshProUGUI>().text = killerName;
        entryTransform.Find("FeedInstance/VictimName").GetComponent<TMPro.TextMeshProUGUI>().text = victimName;

        Destroy(entryTransform.gameObject, 5);
    }
}