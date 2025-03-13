using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TotalMultiManager;
using System.Collections;
using ExitGames.Client.Photon;

public abstract class VoiceManager : MonoBehaviourPunCallbacks
{
    public static VoiceManager Instance { get; private set; } // Singleton 인스턴스

    public GameObject[] players;  // 각 플레이어 GameObject
    public TextMeshProUGUI[] playerTexts;  // 각 플레이어의 TextMeshProUGUI 배열

    public Sprite speakImage;  // 말하는 이미지
    public Sprite defaultImage;  // 기본 이미지
    public Sprite muteImage;  // 기본 이미지

    [SerializeField] private GameObject speakerPanel;

    protected Speaker[] speakers;  // Speaker 컴포넌트를 담을 배열
    protected bool[] isMuted = new bool[4];  // 각 플레이어의 음소거 상태

    protected Recorder recorder;
    // 로컬 플레이어의 자체 음소거 상태 변수
    protected bool selfMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    protected virtual void Start()
    {
        UpdateSpeakersList();
        StartCoroutine(UpdatePlayerTextsBasedOnSpeaker());
    }

    /// <summary>
    /// 매 프레임의 후반부에 호출되는 함수
    /// 스피커 목록을 최신 상태로 유지하기 위해 업데이트 호출
    /// </summary>
    protected virtual void LateUpdate()
    {
        UpdateSpeakersList();
    }

    // 모든 Speaker 컴포넌트를 가져와 speakers 배열 업데이트
    protected abstract void UpdateSpeakersList();

    public void OnClickSpeakerPanel()
    {
        speakerPanel.SetActive(!speakerPanel.activeSelf);
    }

    /// <summary>
    /// 각 스피커의 재생 상태에 따라 플레이어 UI를 업데이트하는 함수
    /// </summary>
    protected void CheckIsPlaying()
    {
        // 모든 플레이어 UI를 비활성화
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(false);
        }

        foreach (var speaker in speakers)
        {
            PhotonView pv = speaker.GetComponent<PhotonView>();
            if (pv == null) continue;

            int index = pv.OwnerActorNr - 1;
            if (index < 0 || index >= players.Length) continue;

            players[index].SetActive(true);

            Image img = playerTexts[index].GetComponentInChildren<Image>();


            // 로컬 플레이어 처리
            if (pv.IsMine)
            {
                // 로컬 플레이어의 AudioSource 가져오기
                AudioSource audioSource = speaker.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    // 본인이 말할 때는 볼륨을 0, 그렇지 않을 때는 1로 설정
                    audioSource.volume = speaker.IsPlaying ? 0f : 1f;
                }
                // 로컬 플레이어의 음소거 여부 및 말하는 상태에 따른 이미지 설정
                img.sprite = selfMuted ? muteImage : (speaker.IsPlaying ? speakImage : defaultImage);

            }
            else
            {
                // 원격 플레이어 처리: 음소거 상태 및 말하는 상태에 따른 이미지 설정
                img.sprite = isMuted[index]
                    ? muteImage
                    : (speaker.IsPlaying ? speakImage : defaultImage);
            }
            // 플레이어 닉네임 업데이트
            playerTexts[index].text = pv.Owner.NickName;
        }
    }

    /// <summary>
    /// 모든 Speaker 컴포넌트를 가진 플레이어를 확인하고 playerTexts를 업데이트
    /// </summary>
    private IEnumerator UpdatePlayerTextsBasedOnSpeaker()
    {
        while (!AllhasTag("HasInfo"))
        {
            yield return null; // 모든 플레이어의 CustomProperties가 준비될 때까지 대기
        }

        foreach (var speaker in speakers)
        {
            PhotonView pv = speaker.GetComponent<PhotonView>();
            if (pv == null) continue;

            int index = pv.OwnerActorNr - 1;
            if (index < 0 || index >= playerTexts.Length) continue;

            players[index].SetActive(true);
            playerTexts[index].text = pv.Owner.NickName;
        }
    }

    /// <summary>
    /// 방에 입장했을 때 호출되는 콜백 함수
    /// 로컬 플레이어의 CustomProperties에 "HasInfo"를 설정
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 로컬 플레이어의 정보가 준비되었음을 표시하는 프로퍼티 설정
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "HasInfo", true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 방에 들어왔을때
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 최신 Speaker 목록 갱신
        UpdateSpeakersList();

        // 입장한 플레이어의 ActorNumber를 인덱스로 사용
        int index = newPlayer.ActorNumber - 1;

        // 인덱스가 유효하면 UI를 활성화하고 닉네임을 업데이트
        if (index >= 0 && index < players.Length)
        {
            players[index].SetActive(true);
            playerTexts[index].text = newPlayer.NickName;
        }
    }

    /// <summary>
    /// 플레이어가 방을 떠났을 때 호출되는 콜백 함수
    /// 해당 플레이어의 UI를 비활성화(또는 닉네임 삭제)하고 스피커 목록을 업데이트
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 나간 플레이어의 ActorNumber를 인덱스로 사용
        int index = otherPlayer.ActorNumber - 1;

        // 인덱스가 유효하면 UI를 비활성화(또는 닉네임을 지움)하여 표시하지 않음
        if (index >= 0 && index < players.Length)
        {
            players[index].SetActive(true);
            playerTexts[index].text = "";
        }
        // 최신 Speaker 목록 갱신
        UpdateSpeakersList();
    }


    /// <summary>
    /// 특정 ActorNumber에 해당하는 플레이어의 음소거 상태를 토글하는 함수
    /// - 로컬 플레이어인 경우 Recorder를 토글하여 자신의 목소리 전송 여부 제어
    /// - 원격 플레이어인 경우 해당 Speaker 컴포넌트의 활성화를 토글하여 클라이언트에서만 음소거 처리
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
                // isMuted 배열에도 반영 (speaker가 비활성화이면 음소거 상태)
                isMuted[index] = !speaker.enabled;

                // UI의 이미지도 즉시 업데이트하여 음소거 상태를 표시
                Image img = playerTexts[index].GetComponentInChildren<Image>();
                img.sprite = isMuted[index] ? muteImage : (speaker.IsPlaying ? speakImage : defaultImage);
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

        // UI 업데이트: 로컬 플레이어 인덱스에 해당하는 이미지 변경
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Image img = playerTexts[index].GetComponentInChildren<Image>();

        img.sprite = selfMuted ? muteImage : defaultImage;
        Debug.Log(img);
        Debug.Log("Self mute toggled: " + (selfMuted ? "Muted" : "Unmuted"));
    }
}
