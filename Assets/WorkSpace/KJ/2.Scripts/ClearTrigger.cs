using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 불(Fire) 태그가 있는 오브젝트가 충돌하면 엔딩씬 실행
        if (other.CompareTag("Fire"))
        {
            LoadEndingScene();
        }
    }

    private void LoadEndingScene()
    {
        // "EndingScene"이라는 이름의 씬을 로드 (씬 이름 변경 가능)
        SceneManager.LoadScene("EndingScene");
    }
}
