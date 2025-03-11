using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapVoice : VoiceManager
{
    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform

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

    // 맵 씬을 벗어나면 VoiceManager를 파괴해줄 함수
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1f);

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "MapScene")
        {
            Destroy(gameObject);
            Debug.Log("Destroy");
        }
    }
}
