using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    [SerializeField] private Transform _greenTeamContainer;
    [SerializeField] private Transform _redTeamContainer;
    [SerializeField] private Transform _entryTemplate;

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

    private void CreateScore(string name, int kills, int deaths, Transform _entryContainer)
    {
        Transform entryTransform = Instantiate(_entryTemplate, _entryContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryTransform.gameObject.SetActive(true);

        string rankString = (entryTransform.GetSiblingIndex() + 1).ToString();

        entryTransform.Find("Rank").GetComponent<TextMesh>().text = rankString;
        entryTransform.Find("Name").GetComponent<TextMesh>().text = name;
        entryTransform.Find("Kills").GetComponent<TextMesh>().text = kills.ToString();
        entryTransform.Find("Deaths").GetComponent<TextMesh>().text = deaths.ToString();
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