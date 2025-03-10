using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class LobbyVoice : VoiceManager
{

    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform

    void Start()
    {
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
        for (int i = 0; i < speakers.Length; i++)
        {
            Debug.Log($"Speaker {i}: {speakers[i].gameObject.name}");
        }

    }
}
