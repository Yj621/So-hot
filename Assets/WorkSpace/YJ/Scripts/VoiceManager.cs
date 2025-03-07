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

    [SerializeField] private Transform playerGroup; // PlayerGroup의 Transform

    [SerializeField] private GameObject speakerPanel;

    private Speaker[] speakers;  // Speaker 컴포넌트를 담을 배열

    private bool[] isSpeakingStatus = new bool[4];  // 각 플레이어의 말하는지 여부 상태 저장 배열
    private bool[] playerExistence = new bool[4];  // 각 플레이어의 존재 여부 상태 저장 배열

    void Start()
    {
        // PlayerGroup 내의 자식들이 잘 초기화되었는지 확인
        Debug.Log($"PlayerGroup: {playerGroup.name}");

        // 플레이어들이 생성된 후에 Speaker 컴포넌트를 찾아 speakers 배열을 초기화
        speakers = new Speaker[0];  // 초기화 상태로 시작
        InitializeSpeakers();
    }

    void LateUpdate()
    {
        /*        // Speaker 배열이 비어있으면 찾고 초기화
                if (speakers == null || speakers.Length == 0)
                {
                    var tempSpeakers = new List<Speaker>();

                    // PlayerGroup 내의 자식 객체와 그 자식들까지 모두 순회
                    foreach (Transform child in playerGroup)
                    {
                        Debug.Log($"Checking child: {child.name}");

                        // 자식 오브젝트와 그 자식들을 순회하면서 Speaker 찾기
                        FindSpeakersRecursively(child, tempSpeakers);
                    }

                    // List에서 배열로 변환하여 speakers 배열에 저장
                    speakers = tempSpeakers.ToArray();
                }
        */

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
            int actorNumber = photonView.OwnerActorNr - 1; // Actor 번호에 맞는 플레이어 존재 여부 설정
            playerExistence[actorNumber] = true;


            // 해당 플레이어의 닉네임을 UI에 업데이트
            string playerNickName = photonView.Owner.NickName;

            // 말하고 있는지 여부에 따라 UI 업데이트
            UpdatePlayerUI(actorNumber + 1, playerExistence[actorNumber], isSpeakingStatus[i], playerNickName);
        }

        // UI 업데이트 (각 플레이어 1~4에 대해)
        for (int i = 0; i < 4; i++)
        {
            // 각 플레이어의 존재 여부 및 말하는지 여부에 따라 UI 업데이트
            UpdatePlayerUI(i + 1, playerExistence[i], isSpeakingStatus[i], null);
        }
    }
    private void InitializeSpeakers()
    {
        var tempSpeakers = new List<Speaker>();

        foreach (Transform child in playerGroup)
        {
            FindSpeakersRecursively(child, tempSpeakers);
        }

        speakers = tempSpeakers.ToArray();
    }

    public override void  OnPlayerEnteredRoom(Player newPlayer)
    {
        InitializeSpeakers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        InitializeSpeakers();
    }

    private void UpdatePlayerUI(int actorNumber, bool isActive, bool isSpeaking, string nickName)
    {
        int index = actorNumber - 1;  // actorNumber에 맞는 인덱스 계산

        if (index < 0 || index >= playerTexts.Length) return;  // 인덱스 범위 체크

        // 말하는지 여부에 따라 이미지 설정
        playerTexts[index].GetComponentInChildren<Image>().sprite = isSpeaking ? speakImage : defaultImage;
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


        // 리스트 내부 요소를 순회하며 출력
        for (int i = 0; i < speakersList.Count; i++)
        {
            speaker = speakersList[i];
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
}
