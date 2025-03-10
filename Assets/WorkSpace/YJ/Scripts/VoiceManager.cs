using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceManager : MonoBehaviourPunCallbacks
{

    public Sprite speakImage;  // 말하는 이미지
    public Sprite defaultImage;  // 기본 이미지
    public Sprite muteImage;  // 기본 이미지

    [SerializeField] private GameObject speakerPanel;

    protected Speaker[] speakers;  // Speaker 컴포넌트를 담을 배열
    protected bool[] isMuted = new bool[4];  // 각 플레이어의 음소거 상태

    protected Recorder recorder;
    // 로컬 플레이어의 자체 음소거 상태 변수
    protected bool selfMuted = false;

    public static VoiceManager Instance;
    // Awake()에서 싱글턴 체크 후, 중복 객체 제거
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Start()
    {
        UpdateSpeakersList();
        recorder = GetComponent<Recorder>();
    }

    protected virtual void LateUpdate()
    {
        // Speaker 목록 갱신
        UpdateSpeakersList();

    }

    // 모든 Speaker 컴포넌트를 가져와 speakers 배열 업데이트
    protected void UpdateSpeakersList()
    {
        speakers =  FindObjectsOfType<Speaker>(true);
    }

    public void OnClickSpeakerPanel()
    {
        speakerPanel.SetActive(!speakerPanel.activeSelf);
    }


    /// <summary>
    /// 특정 ActorNumber에 해당하는 플레이어의 음소거 상태를 토글,
    /// 본인(로컬 플레이어)인 경우에는 Recorder를 토글하여 자신의 목소리가 다른 사람에게 전달 X,
    /// 원격 플레이어인 경우에는 해당 Speaker를 비활성화하여 내 클라이언트에서만 들리지 않게 함
    /// </summary>
    /// <param name="actorNumber">음소거할 플레이어의 ActorNumber</param>
    public void ToggleSpeaker(int actorNumber)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            ToggleSelfMute();
            return;
        }

        // ActorNumber를 인덱스로 변환 (배열은 0부터 시작)
        int index = actorNumber - 1;
        foreach (var speaker in speakers)
        {
            PhotonView pv = speaker.GetComponent<PhotonView>();
            // 해당 Speaker의 소유자와 전달된 ActorNumber가 일치하면 음소거 상태를 토글
            if (pv != null && pv.OwnerActorNr == actorNumber)
            {
                // Speaker 컴포넌트의 활성화 여부를 반전시킴 (비활성화되면 음소거)
                speaker.enabled = !speaker.enabled;
                return;
            }
        }
    }
    /// <summary>
    /// 로컬 플레이어의 음소거를 토글
    /// </summary>
    private void ToggleSelfMute()
    {
        if (recorder == null)
        {
            Debug.LogWarning("Recorder is not assigned!");
            return;
        }

        // selfMuted 상태 반전
        selfMuted = !selfMuted;
        // 음소거 상태이면 전송하지 않음, 아니면 전송
        // TransmitEnabled : 시작하자마자 말하기가 가능함(눌러서 말하기 제어 가능)
        recorder.TransmitEnabled = !selfMuted;

        Debug.Log("Self mute toggled: " + (selfMuted ? "Muted" : "Unmuted"));
    }
}
