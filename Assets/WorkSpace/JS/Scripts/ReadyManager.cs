using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public Donghyun.Network.NetWorkManager networkManager;

    public GameObject[] p1Characters; // P1 캐릭터 배열
    public GameObject[] p2Characters; // P2 캐릭터 배열
    public GameObject[] p3Characters; // P3 캐릭터 배열
    public GameObject[] p4Characters; // P4 캐릭터 배열

    public GameObject[] blackCharacters;

    public GameObject[] characterSlots; // 1P~4P의 UI 슬롯
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button readyButton;
    public Button cancelButton;

    public Sprite[] characterImages;
    public Image characterDisplay;
    public Sprite unknownCharacterSprite;

    private Dictionary<int, int> playerCharacterIndex = new Dictionary<int, int>(); // 플레이어별 캐릭터 선택 인덱스
    private Dictionary<int, bool> playerReadyState = new Dictionary<int, bool>(); // 플레이어별 Ready 상태
    private Dictionary<int, GameObject> currentCharacters = new Dictionary<int, GameObject>();

    private int currentPlayerSlot = 0; // 현재 버튼이 조작 중인 플레이어 슬롯
    private int mySlot = 0;

    private void Start()
    {
     

        SetupUI();
        BindButtonEvents();
    }

    public void UpdateSlotUI(int slot, Player player)
    {
        if (player == PhotonNetwork.LocalPlayer)
        {
            mySlot = slot;
        }

        characterSlots[slot].SetActive(true);
        UpdateCharacterDisplay();
    }




    private void SetupUI()
    {
        mySlot = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber - 1을 mySlot으로 설정
        Debug.Log($"[ReadyManager] {PhotonNetwork.LocalPlayer.NickName}의 PlayerSlot 설정 완료: {mySlot}");

        if (!playerCharacterIndex.ContainsKey(mySlot))
        {
            playerCharacterIndex[mySlot] = 0; // 기본 캐릭터 인덱스 설정
        }

        blackCharacters[mySlot].SetActive(true);
        UpdateCharacterDisplay();
    }


    private void BindButtonEvents()
    {
        Debug.Log($"[ReadyManager] BindButtonEvents() 실행됨 (현재 mySlot: {mySlot})");

        if (leftArrowButton == null || rightArrowButton == null || readyButton == null || cancelButton == null)
        {
            Debug.LogError("[ReadyManager] 버튼이 할당되지 않았습니다! Inspector에서 확인하세요.");
            return;
        }

        // 기존 이벤트 제거 후 다시 등록 (중복 방지)
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        readyButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        leftArrowButton.onClick.AddListener(() =>
        {
            Debug.Log($"[ReadyManager] 왼쪽 버튼 클릭 (mySlot: {mySlot})");
            OnLeftArrow();
        });

        rightArrowButton.onClick.AddListener(() =>
        {
            Debug.Log($"[ReadyManager] 오른쪽 버튼 클릭 (mySlot: {mySlot})");
            OnRightArrow();
        });

        readyButton.onClick.AddListener(() =>
        {
            Debug.Log($"[ReadyManager] 레디 버튼 클릭 (mySlot: {mySlot})");
            OnSelect(mySlot);
        });

        cancelButton.onClick.AddListener(() =>
        {
            Debug.Log($"[ReadyManager] 취소 버튼 클릭 (mySlot: {mySlot})");
            OnCancel(mySlot);
        });

        Debug.Log("[ReadyManager] 모든 버튼 이벤트가 성공적으로 등록됨.");
    }



    public void OnLeftArrow()
    {
        Debug.Log("1");
        if (!CanControl(mySlot))
        {
            Debug.Log("2");
            return;
        }

        if (!playerCharacterIndex.ContainsKey(mySlot))
        {
            Debug.Log("3");
            playerCharacterIndex[mySlot] = 0;
        }

        switch (mySlot)
        {
            case 0: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] - 1 + p1Characters.Length) % p1Characters.Length; break;
            case 1: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] - 1 + p2Characters.Length) % p2Characters.Length; break;
            case 2: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] - 1 + p3Characters.Length) % p3Characters.Length; break;
            case 3: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] - 1 + p4Characters.Length) % p4Characters.Length; break;
        }

        UpdateCharacterDisplay();
        photonView.RPC("RPC_UpdateCharacter", RpcTarget.AllBuffered, mySlot, playerCharacterIndex[mySlot]);
    }

    public void OnRightArrow()
    {
        if (!CanControl(mySlot)) return;

        if (!playerCharacterIndex.ContainsKey(mySlot))
        {
            playerCharacterIndex[mySlot] = 0;
        }

        switch (mySlot)
        {
            case 0: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] + 1) % p1Characters.Length; break;
            case 1: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] + 1) % p2Characters.Length; break;
            case 2: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] + 1) % p3Characters.Length; break;
            case 3: playerCharacterIndex[mySlot] = (playerCharacterIndex[mySlot] + 1) % p4Characters.Length; break;
        }

        UpdateCharacterDisplay();
        photonView.RPC("RPC_UpdateCharacter", RpcTarget.AllBuffered, mySlot, playerCharacterIndex[mySlot]);
    }


    public void OnSelect(int slot)
    {
        if (!CanControl(slot)) return;

        playerReadyState[slot] = true;
        photonView.RPC("RPC_SetReady", RpcTarget.AllBuffered, slot, true);
    }

    public void OnCancel(int slot)
    {
        if (!CanControl(slot)) return;

        playerReadyState[slot] = false;
        photonView.RPC("RPC_SetReady", RpcTarget.AllBuffered, slot, false);
    }

    [PunRPC]
    private void RPC_UpdateCharacter(int slot, int index)
    {
        playerCharacterIndex[slot] = index;
        UpdateCharacterDisplay();
    }

    [PunRPC]
    private void RPC_SetReady(int slot, bool isReady)
    {
        playerReadyState[slot] = isReady;
    }

    private void UpdateCharacterDisplay()
    {
        for (int slot = 0; slot < 4; slot++) // 1P~4P 갱신
        {
            if (!playerCharacterIndex.ContainsKey(slot))
            {
                playerCharacterIndex[slot] = 0; // 기본값 설정
            }

            bool isSlotEmpty = !PhotonNetwork.PlayerList.Any(p => p.ActorNumber - 1 == slot);
            blackCharacters[slot].SetActive(isSlotEmpty);

            if (currentCharacters.ContainsKey(slot) && currentCharacters[slot] != null)
            {
                currentCharacters[slot].SetActive(false);
            }

            GameObject selectedCharacter = null;
            if (!isSlotEmpty && playerCharacterIndex.ContainsKey(slot))
            {
                switch (slot)
                {
                    case 0: selectedCharacter = p1Characters[playerCharacterIndex[slot]]; break;
                    case 1: selectedCharacter = p2Characters[playerCharacterIndex[slot]]; break;
                    case 2: selectedCharacter = p3Characters[playerCharacterIndex[slot]]; break;
                    case 3: selectedCharacter = p4Characters[playerCharacterIndex[slot]]; break;
                }
            }

            if (selectedCharacter != null)
            {
                selectedCharacter.SetActive(true);
                currentCharacters[slot] = selectedCharacter;
            }

            characterDisplay.sprite = (playerCharacterIndex.ContainsKey(mySlot) && playerCharacterIndex[mySlot] != -1)
                ? characterImages[playerCharacterIndex[mySlot]]
                : unknownCharacterSprite;
        }
    }


    public void ChangeControlToNextPlayer()
    {
        currentPlayerSlot = (currentPlayerSlot + 1) % 4;
        Debug.Log($"현재 조작 중인 플레이어 슬롯: {currentPlayerSlot + 1}P");

        // 모든 클라이언트에서 `currentPlayerSlot` 변경
        photonView.RPC("RPC_ChangeControl", RpcTarget.AllBuffered, currentPlayerSlot);
    }

    private bool CanControl(int slot)
    {
        return slot == PhotonNetwork.LocalPlayer.ActorNumber - 1;
    }

    [PunRPC]
    private void RPC_ChangeControl(int newSlot)
    {
        currentPlayerSlot = newSlot;
        UpdateCharacterDisplay();
    }
}
