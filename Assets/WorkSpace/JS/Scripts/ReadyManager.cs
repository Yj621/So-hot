using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ReadyManager : MonoBehaviour
{
    public GameObject[] characters; //캐릭터 변환
    public Sprite[] characterImages; //캐릭터 이미지 모음
    public Image characterImage; //이미지를 꽂을 곳
    private int currentIndex = -1; //현재 인덱스
    public Sprite unknownCharacterSprite; // ??? 이미지 (선택 해제 상태)
    public GameObject blackCharacter; // 선택되지 않은 상태에서 보여줄 검은색 캐릭터
    private GameObject currentCharacter;

    private Dictionary<int, int> playerSelections = new Dictionary<int, int>();

    private PhotonView pv;

    public GameObject[] characterPrefabs; // 캐릭터 프리팹 목록
    public Transform[] spawnPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pv = GetComponent<PhotonView>();
        ResetSelection(); //들어갈 때 리셋하고 시작
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLeftArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex -1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay(); //캐릭터 순서 정렬하고 업뎃
    }

    public void OnRightArrow()
    {
        if (!pv.IsMine) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacterDisplay(); //똑같이 업뎃

        pv.RPC("RPCUpdateCharacterSelection", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentIndex);
    }

    public void OnSelect()
    {
        if (currentIndex == -1) return; // 아무 캐릭터도 선택 안 되어 있을 때 예외 처리
        Debug.Log("선택된 캐릭터: " + characters[currentIndex].name);
        pv.RPC("RPCConfirmCharacterSelection", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentIndex);
    }

    public void OnCancel()
    {
        ResetSelection(); //취소하면 다시 검은색으로 돌아감
    }

    private void UpdateCharacterDisplay()
    {
        // 이전 캐릭터 비활성화
        blackCharacter.SetActive(false);
        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        // 새로운 캐릭터 활성화
        currentCharacter = characters[currentIndex];
        currentCharacter.SetActive(true);

        // 초상화 업데이트
        characterImage.sprite = characterImages[currentIndex];
    }


    private void ResetSelection()
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);//남은 거 다 지우고
        } 
        blackCharacter.SetActive(true); //검은캐릭터 올리기
        currentIndex = -1; //인덱스는 다시 설정 불가한걸로
        characterImage.sprite = unknownCharacterSprite; //캐릭터 이미지 불명이미지
    }

    [PunRPC]
    private void RPCUpdateCharacterSelection(int actorNumber, int selectedIndex)
    {
        if (playerSelections.ContainsKey(actorNumber))
        {
            playerSelections[actorNumber] = selectedIndex; 
            //actorIndex 번호의 캐릭터 선택은 selectedIndex 번의 캐릭터이다
        }
        else
        {
            playerSelections.Add(actorNumber, selectedIndex);
            //키를 가지지 않았다면 저걸 추기해준다
        }
    }

    [PunRPC]
    private void RPCConfirmSelections(int actorNumber, int selectdIndex)
    {

    }
}
