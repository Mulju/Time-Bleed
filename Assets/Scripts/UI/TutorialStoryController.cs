using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialStoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPiece;
    [SerializeField] private GameObject nextTextPiece;

    private void OnEnable()
    {
        StartCoroutine(IncreaseContrast());
    }

    IEnumerator IncreaseContrast()
    {
        while(textPiece.color.a < 1)
        {
            Color prevColor = textPiece.color;
            textPiece.color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a + 0.01f);
            yield return new WaitForSeconds(0.03f);
        }

        if (nextTextPiece != null)
        {
            nextTextPiece.SetActive(true);
        }
    }
}
