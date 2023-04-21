using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

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

    private void CreateScore(string name, int kills, int deaths, Transform entryContainer)
    {
        Transform entryTransform = Instantiate(_entryTemplate, entryContainer);
        entryTransform.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(entryContainer.GetComponent<RectTransform>());

        string rankString = (entryContainer.childCount / _entryTemplate.childCount).ToString();

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