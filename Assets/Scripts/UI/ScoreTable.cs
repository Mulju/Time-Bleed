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

    public void UpdateScore(string name, int kills, int deaths, int teamTag, int rank)
    {
        if (teamTag == 0)
        {
            CreateScore(name, kills, deaths, _redTeamContainer, rank)
        }
        else
        {
            CreateScore(name, kills, deaths, _greenTeamContainer, rank)
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
        string timeText;

        float greenMinutes = _matchManager.greenClock.remainingMinutes;
        float greenSeconds = _matchManager.greenClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.greenClock.remainingSeconds);

        timeText = (greenMinutes < 10 ? "0" + greenMinutes + ":" : greenMinutes + ":");

        _greenTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = timeText;

        timeText = (greenSeconds < 10 ? "0" + greenSeconds : greenSeconds.ToString());

        _greenTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = timeText;


        float redMinutes = _matchManager.redClock.remainingMinutes;
        float redSeconds = _matchManager.redClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.redClock.remainingSeconds);

        timeText = (redMinutes < 10 ? "0" + redMinutes + ":" : redMinutes + ":");

        _redTime.Find("Minutes").GetComponent<TMPro.TextMeshProUGUI>().text = timeText;

        timeText = (redSeconds < 10 ? "0" + redSeconds : redSeconds.ToString());

        _redTime.Find("Seconds").GetComponent<TMPro.TextMeshProUGUI>().text = timeText;
    }

    public void UpdateBoard(int redKills, int greenKills)
    {
        _greenTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = greenKills.ToString();
        _redTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = redKills.ToString();
    }

    private void CreateScore(string name, int kills, int deaths, Transform entryContainer, int rank)
    {
        Transform entryTransform = Instantiate(_entryTemplate, entryContainer);
        entryTransform.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(entryContainer.GetComponent<RectTransform>());

        string rankString = rank.ToString();

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