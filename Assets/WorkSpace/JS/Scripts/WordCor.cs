using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordCor : MonoBehaviour
{
    public TextMeshProUGUI[] Word;
    public Button[] Buttons;


    void Start()
    {
        for (int i = 0; i < Word.Length; i++)
        {
            Word[i].gameObject.SetActive(false);
            Buttons[i].gameObject.SetActive(false);
        }
        
        StartCoroutine(WordCome());
    }

    public IEnumerator WordCome()
    {
        yield return new WaitForSeconds(8f);

        for (int i = 0; i < Word.Length; i++)
        {
            Word[i].gameObject.SetActive(true);
            Buttons[i].gameObject.SetActive(true);
        }
        float time = GameManager.Instance.GetElapsedTime();
        Word[1].text = time.ToString("F2") + " 초 걸렸다!";
    }

    public void ExitEnding()
    {
        GameManager.Instance.ResetTimer();
        SceneManager.LoadScene("StartScene");
    }

    public void Exit()
    {
        GameManager.Instance.ResetTimer();
        Application.Quit();
    }
}
