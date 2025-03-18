using System.Collections;
using TMPro;
using UnityEngine;
using static TotalMultiManager;

public class Loading : MonoBehaviour
{
    [SerializeField] CircleTransition circleTransition;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] private TMP_Text loadingText;

    private void Awake()
    {
        LoadingPanel.SetActive(true);
        circleTransition.gameObject.SetActive(true);

        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        int count = 0;
        while (!AllhasTag("setPlayerGroup"))
        {
            if (count == 0)
            {
                loadingText.text = "기다리는 중";
            }
            else
            {
                loadingText.text += ".";
            }
            count = (count + 1) % 4;
            yield return new WaitForSeconds(0.1f);
        }

        int totalLoadingCount = 0; 
        while (totalLoadingCount < 50)
        {
            if (count == 0)
            {
                loadingText.text = "기다리는 중";
            }
            else
            {
                loadingText.text += ".";
            }
            count = (count + 1) % 4;
            totalLoadingCount++;
            yield return new WaitForSeconds(0.1f);
        }


        LoadingPanel.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        circleTransition.FadeIn();
    }
}
