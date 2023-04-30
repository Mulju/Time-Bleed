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

    public void UpdateScore(string name, int kills, int deaths, int time, int teamTag, int rank)
    {
        if (teamTag == 0)
        {
            CreateScore(name, kills, deaths, time, _redTeamContainer, rank);
        }
        else
        {
            CreateScore(name, kills, deaths, time, _greenTeamContainer, rank);
        }
    }

    private void Update()
    {
        UpdateTime();

        if (_matchManager.currentMatchState == MatchManager.MatchState.STARTING)
        {
            UpdateBoard(0, 0);
        }
    }

    public void UpdateTime()
    {
        float greenMinutes = _matchManager.greenClock.remainingMinutes;
        float greenSeconds = _matchManager.greenClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.greenClock.remainingSeconds);

        string timeMinText = (greenMinutes < 10 ? "0" + greenMinutes + ":" : greenMinutes + ":");
        string timeSecText = (greenSeconds < 10 ? "0" + greenSeconds : greenSeconds.ToString());

        _greenTime.GetComponent<TMPro.TextMeshProUGUI>().text = timeMinText + timeSecText;


        float redMinutes = _matchManager.redClock.remainingMinutes;
        float redSeconds = _matchManager.redClock.remainingSeconds == 0 ? 0 : Mathf.Floor(_matchManager.redClock.remainingSeconds);

        timeMinText = (redMinutes < 10 ? "0" + redMinutes + ":" : redMinutes + ":");
        timeSecText = (redSeconds < 10 ? "0" + redSeconds : redSeconds.ToString());

        _redTime.GetComponent<TMPro.TextMeshProUGUI>().text = timeMinText + timeSecText;
    }

    public void UpdateBoard(int redKills, int greenKills)
    {
        // _greenTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = greenKills.ToString();
        // _redTotal.Find("InfoBox/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = redKills.ToString();
    }

    private void CreateScore(string name, int kills, int deaths, int time, Transform entryContainer, int rank)
    {
        Transform entryTransform = Instantiate(_entryTemplate, entryContainer);
        entryTransform.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(entryContainer.GetComponent<RectTransform>());

        entryTransform.Find("InfoBox/Rank").GetComponent<TMPro.TextMeshProUGUI>().text = rank.ToString();
        entryTransform.Find("InfoBox/Name").GetComponent<TMPro.TextMeshProUGUI>().text = name;
        entryTransform.Find("InfoBox2/Kills").GetComponent<TMPro.TextMeshProUGUI>().text = kills.ToString();
        entryTransform.Find("InfoBox2/Deaths").GetComponent<TMPro.TextMeshProUGUI>().text = deaths.ToString();

        // if (infobox/time + time >= 60)
        int stoleTime = int.TryParse(entryTransform.Find("InfoBox2/Time").GetComponent<TMPro.TextMeshProUGUI>().text, out stoleTime) ? stoleTime : 0;

        // break up time into minutes and seconds, 01:37
        if (stoleTime + time >= 60)
        {
            int minutes = (stoleTime + time) / 60;
            int seconds = (stoleTime + time) % 60;

            string timeMinText = (minutes < 10 ? "0" + minutes + ":" : minutes + ":");
            string timeSecText = (seconds < 10 ? "0" + seconds : seconds.ToString());

            entryTransform.Find("InfoBox2/Time").GetComponent<TMPro.TextMeshProUGUI>().text = timeMinText + timeSecText;
        }
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