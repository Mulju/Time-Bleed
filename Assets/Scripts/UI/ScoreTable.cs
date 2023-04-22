using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class ScoreTable : MonoBehaviour
{
    [SerializeField] private Transform _greenTeamContainer;
    [SerializeField] private Transform _redTeamContainer;
    [SerializeField] private Transform _entryTemplate;
    [SerializeField] private Transform _greenTotal;
    [SerializeField] private Transform _redTotal;
    [SerializeField] private Transform _greenTime;
    [SerializeField] private Transform _redTime;

    MatchManager _matchManager;

    private void Awake()
    {
        _matchManager = MatchManager.matchManager;
    }

    public void UpdateScore(string name, int kills, int deaths, int teamTag)
    {
        if (teamTag == 0)
        {
            CreateScore(name, kills, deaths, _redTeamContainer);
        }
        else
        {
            CreateScore(name, kills, deaths, _greenTeamContainer);
        }
    }

    private void Update()
    {
        if (_matchManager.currentMatchState == MatchManager.MatchState.IN_PROGRESS)
        {
            UpdateTime();
        }
    }

    public void UpdateTime()
    {
        float greenMinutes = _matchManager.greenClock.remainingMinutes;
        float greenSeconds = _matchManager.greenClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.greenClock.remainingSeconds);

        if (greenMinutes < 10)
        {
            _greenTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = "0" + greenMinutes + ":";
        }
        else
        {
            _greenTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = greenMinutes + ":";
        }

        if (greenSeconds < 10)
        {
            _greenTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = "0" + greenSeconds;
        }
        else
        {
            _greenTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = greenSeconds.ToString();
        }

        float redMinutes = _matchManager.redClock.remainingMinutes;
        float redSeconds = _matchManager.redClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.redClock.remainingSeconds);

        if (redMinutes < 10)
        {
            _redTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = "0" + redMinutes + ":";
        }
        else
        {
            _redTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = redMinutes + ":";
        }

        if (redSeconds < 10)
        {
            _redTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = "0" + redSeconds;
        }
        else
        {
            _redTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = redSeconds.ToString();
        }
    }

    public void UpdateBoard(int redKills, int greenKills)
    {
        _greenTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = greenKills.ToString();
        _redTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = redKills.ToString();
    }

    private void CreateScore(string name, int kills, int deaths, Transform entryContainer)
    {
        Transform entryTransform = Instantiate(_entryTemplate, entryContainer);
        entryTransform.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(entryContainer.GetComponent<RectTransform>());

        string rankString;
        int rank = 1; // plöp
        rankString = rank.ToString();

        entryTransform.Find("InfoBox/Rank").GetComponent<TMPro.TextMeshProUGUI>().text = rankString;
        entryTransform.Find("InfoBox/Name").GetComponent<TMPro.TextMeshProUGUI>().text = name;
        entryTransform.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = kills.ToString();
        entryTransform.Find("InfoBox/Deaths").GetComponent<TMPro.TextMeshProUGUI>().text = deaths.ToString();
    }

    public void DestroyScores()
    {
        foreach (Transform child in _greenTeamContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _redTeamContainer)
        {
            Destroy(child.gameObject);
        }
    }
}