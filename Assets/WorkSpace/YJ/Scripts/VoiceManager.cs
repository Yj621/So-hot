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

    private bool[] isSpeakingStatus = new bool[4];  // 각 플레이어의 말하는지 여부 상태 저장 배열
    private bool[] playerExistence = new bool[4];  // 각 플레이어의 존재 여부 상태 저장 배열
    private bool[] isMuted = new bool[4];  // 각 플레이어의 음소거 상태



    void Start()
    {
        // 플레이어들이 생성된 후에 Speaker 컴포넌트를 찾아 speakers 배열을 초기화
        speakers = new Speaker[0];  // 초기화 상태로 시작
        
        InitializeSpeakers();
    }

    void LateUpdate()
    {
        InitializeSpeakers();

        // 각 플레이어가 Speaker를 가지고 있는지 여부를 체크
        for (int i = 0; i < speakers.Length; i++)
        {
            var speaker = speakers[i];
            if (speaker == null || speaker.GetComponent<PhotonView>() == null)
            {
                Debug.LogWarning("Speaker가 없거나 PhotonView가 없음");
                continue;
            }

            // 말을 하고 있는지 여부를 isSpeakingStatus 배열에 저장
            isSpeakingStatus[i] = speaker.IsPlaying;

            // 해당 플레이어의 존재 여부를 playerExistence 배열에 설정
            var photonView = speaker.GetComponent<PhotonView>();
            int actorNumber = photonView.OwnerActorNr - 1;  // Actor 번호에 맞는 플레이어 존재 여부 설정
            playerExistence[i] = true; // 플레이어 존재 여부 설정

            // UI 업데이트
            string playerNickName = photonView.Owner.NickName; // 각 플레이어의 닉네임을 가져옴

            // actorNumber에 맞게 UI 업데이트
            UpdatePlayerUI(actorNumber + 1, playerExistence[i], isSpeakingStatus[i], playerNickName);
        }

        // 자기 자신에 대해서도 UI 업데이트
        UpdatePlayerUI(PhotonNetwork.LocalPlayer.ActorNumber, playerExistence[PhotonNetwork.LocalPlayer.ActorNumber - 1], isSpeakingStatus[PhotonNetwork.LocalPlayer.ActorNumber - 1], PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        InitializeSpeakers();
        // 새로운 플레이어가 입장했을 때 UI를 업데이트하려면 새로운 플레이어의 PhotonView를 찾아 UI를 업데이트
        string newPlayerNickName = newPlayer.NickName;
        int newPlayerActorNumber = newPlayer.ActorNumber;
        UpdatePlayerUI(newPlayerActorNumber, true, false, newPlayerNickName); // isSpeaking은 false로 설정
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        InitializeSpeakers();
        // 플레이어가 나갔을 때 해당 플레이어의 UI를 비활성화
        int leftPlayerActorNumber = otherPlayer.ActorNumber;
        UpdatePlayerUI(leftPlayerActorNumber, false, false, "");
    }

    private void InitializeSpeakers()
    {
        var tempSpeakers = new List<Speaker>();

        foreach (Transform child in playerGroup)
        {
            FindSpeakersRecursively(child, tempSpeakers);
        }

        speakers = tempSpeakers.ToArray();

        // 존재하지 않는 플레이어의 UI 비활성화
        for (int i = 0; i < 4; i++)
        {
            Debug.Log($"playerExistence[i] : {playerExistence[i]}");
            if (!playerExistence[i])
            {
                players[i].SetActive(false);  // 플레이어 활성화 여부
            }
        }
    }


    private void UpdatePlayerUI(int actorNumber, bool isActive, bool isSpeaking, string nickName)
    {
        int index = actorNumber - 1;  // actorNumber에 맞는 인덱스 계산

        if (index < 0 || index >= playerTexts.Length) return;  // 인덱스 범위 체크

        // 음소거 상태인지 확인
        if (isMuted[index])
        {
            // 음소거 상태일 경우 muteImage로 설정
            playerTexts[index].GetComponentInChildren<Image>().sprite = muteImage;
        }
        else
        {
            // 음소거가 아닐 경우 말하는지 여부에 따라 이미지 설정
            playerTexts[index].GetComponentInChildren<Image>().sprite = isSpeaking ? speakImage : defaultImage;
        }

        players[index].SetActive(isActive);  // 플레이어 활성화 여부

        if (!string.IsNullOrEmpty(nickName))
        {
            playerTexts[index].text = nickName;  // 닉네임 업데이트
        }
    }

    // Speaker 컴포넌트를 재귀적으로 찾는 함수
    void FindSpeakersRecursively(Transform parent, List<Speaker> speakersList)
    {
        // 현재 부모 오브젝트에서 Speaker 찾기
        var speaker = parent.GetComponent<Speaker>();
        if (speaker != null)
        {
            speakersList.Add(speaker);
        }


        // 자식 오브젝트들이 있다면 그 자식들을 재귀적으로 탐색
        foreach (Transform child in parent)
        {
            FindSpeakersRecursively(child, speakersList);
        }
    }
    public void OnClickSpeakerPanel()
    {
        speakerPanel.SetActive(!speakerPanel.activeSelf);
    }

    //음소거
    public void ToggleSpeaker(int actorNumber)
    {
        foreach (var speaker in speakers)
        {
            var photonView = speaker.GetComponent<PhotonView>();
            if (photonView != null && photonView.OwnerActorNr == actorNumber)
            {
                int index = actorNumber - 1;  // 배열 인덱스 계산

                // Speaker 활성화/비활성화 및 음소거 상태 업데이트
                speaker.enabled = !speaker.enabled;
                isMuted[index] = !speaker.enabled;  // speaker.enabled가 false면 음소거 상태로 설정
               
                // UI 업데이트 즉시 호출
                UpdatePlayerUI(actorNumber, playerExistence[index], isSpeakingStatus[index], photonView.Owner.NickName);

                Debug.Log($"Speaker for Actor {actorNumber} is now {(speaker.enabled ? "enabled" : "disabled")}");
                return;
            }
        }
    }
}
