using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] players;  // 각 플레이어 GameObject
    [SerializeField] private TextMeshProUGUI[] playerTexts;  // 각 플레이어의 TextMeshProUGUI 배열

    [SerializeField] private Sprite speakImage;  // 말하는 이미지
    [SerializeField] private Sprite defaultImage;  // 기본 이미지
    [SerializeField] private Sprite muteImage;  // 기본 이미지

    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform

    [SerializeField] private GameObject speakerPanel;

    private Speaker[] speakers;  // Speaker 컴포넌트를 담을 배열
    private bool[] isMuted = new bool[4];  // 각 플레이어의 음소거 상태

    private Recorder recorder;
    // 로컬 플레이어의 자체 음소거 상태 변수
    private bool selfMuted = false;

    void Start()
    {
        UpdateSpeakersList();
    }

    void LateUpdate()
    {
        // Speaker 목록 갱신
        UpdateSpeakersList();

        // 우선 모든 플레이어 UI를 비활성화
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(false);
        }

        //각 Speaker 정보로 UI 업데이트
        foreach (var speaker in speakers)
        {
            // speaker에게서 PhotonView 찾기
            PhotonView pv = speaker.GetComponent<PhotonView>();

            if (pv == null)
            {
                continue;
            }

            // PhotonView의 소유자(플레이어)의 ActorNumber를 인덱스로 사용 (배열은 0부터 시작하므로 -1)
            int index = pv.OwnerActorNr - 1;
            
            // 인덱스가 올바르지 않으면 다음 Speaker로 넘어감
            if (index < 0 || index >= players.Length)
            {
                continue;
            }

            //플레이어 UI 활성화
            players[index].SetActive(true);

            // 플레이어 UI의 자식 Image 컴포넌트를 가져옴 (말하는 상태 또는 음소거 상태에 따른 이미지 변경을 위해)
            Image img = playerTexts[index].GetComponentInChildren<Image>();
         
            // 음소거 상태인 경우, 음소거 이미지를 설정
            if (isMuted[index])
            {
                img.sprite = muteImage;
            }
            else
            {
                img.sprite = speaker.IsPlaying ? speakImage : defaultImage;
            }

            //닉네임 업데이트
            playerTexts[index].text = pv.Owner.NickName;
        }

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

    //방에서 떠났을때
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

    // playerGroup 내의 모든 Speaker 컴포넌트를 가져와 speakers 배열 업데이트
    void UpdateSpeakersList()
    {
        speakers = playerGroup.GetComponentsInChildren<Speaker>(true);
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
        if(actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
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

        Debug.Log("Self mute toggled: " + (selfMuted ? "Muted" : "Unmuted"));
    }
}
