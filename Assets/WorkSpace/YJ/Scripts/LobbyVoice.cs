using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyVoice : VoiceManager
{
    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform

    public static LobbyVoice Instance { get; private set; } // Singleton 인스턴스

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void LateUpdate()
    {
        UpdateSpeakersList();
        CheckIsPlaying();
    }

    // playerGroup 내의 모든 Speaker 컴포넌트를 가져와 speakers 배열 업데이트
    protected override void UpdateSpeakersList()
    {
        speakers = playerGroup.GetComponentsInChildren<Speaker>(true);
    }

    public void DestroyVoiceManager()
    {
        //StartCoroutine(DelayDestroy());
    }

    // 방을 나간 플레이어가 있을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        DestroyVoiceManager();
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1f);

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "LobbyScene")
        {
            Destroy(gameObject);
            Debug.Log("Destroy");
        }
    }
}
