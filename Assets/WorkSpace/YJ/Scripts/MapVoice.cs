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
    // 방을 나간 플레이어가 있을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        StartCoroutine(DelayDestroy());
    }

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

    // 맵씬에서는 매 프레임마다 "Player" 태그를 가진 오브젝트를 찾아 UI를 갱신합니다.
    protected override void LateUpdate()
    {
        UpdateSpeakersList();
        // UI 업데이트 로직 호출
        CheckIsPlaying();
    }

    protected override void UpdateSpeakersList()
    {
        // "Player" 태그를 가진 모든 오브젝트를 검색합니다.
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        List<Speaker> foundSpeakers = new List<Speaker>();

        // 각 오브젝트에서 Speaker 컴포넌트를 자식에서 찾습니다.
        foreach (GameObject playerObj in playerObjects)
        {
            Speaker sp = playerObj.GetComponentInChildren<Speaker>();
            if (sp != null)
            {
                foundSpeakers.Add(sp);
            }
        }
        speakers = foundSpeakers.ToArray();
    }


}
