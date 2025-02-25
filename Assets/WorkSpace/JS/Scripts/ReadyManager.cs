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
    public GameObject[] characterSlots; // 1P~4P UI 슬롯
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button readyButton;
    public Button cancelButton;

    public Sprite[] characterImages;
    public Image characterDisplay;
    public Sprite unknownCharacterSprite;

    private Dictionary<int, int> playerCharacterIndex = new Dictionary<int, int>(); // 각 슬롯별 캐릭터 선택 인덱스
    private Dictionary<int, bool> playerReadyState = new Dictionary<int, bool>(); // 각 슬롯별 Ready 상태
    private Dictionary<int, GameObject> currentCharacters = new Dictionary<int, GameObject>();

    private int currentPlayerSlot = 0; // 현재 조작 중인 슬롯
    private int mySlot = 0;

    private void Start()
    {
        SetupUI();
        BindButtonEvents();
    }

    public void UpdateSlotUI(int slot, Player player)
    {
        if (player == PhotonNetwork.LocalPlayer)
            mySlot = slot;
        characterSlots[slot].SetActive(true);
        UpdateCharacterDisplay();
    }

    private void SetupUI()
    {
        // 내 CustomProperties에 PlayerSlot이 없다면 빈 슬롯 할당
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerSlot"))
        {
            Debug.LogError("[ReadyManager] PlayerSlot이 설정되지 않음! 기본값을 설정합니다.");
            HashSet<int> usedSlots = new HashSet<int>();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("PlayerSlot"))
                    usedSlots.Add((int)player.CustomProperties["PlayerSlot"]);
            }
            for (int i = 0; i < 4; i++)
            {
                if (!usedSlots.Contains(i))
                {
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props["PlayerSlot"] = i;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    mySlot = i;
                    Debug.Log($"[ReadyManager] {PhotonNetwork.LocalPlayer.NickName}에게 PlayerSlot {i} 할당됨.");
                    break;
                }
            }
        }
        else
        {
            mySlot = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"];
        }

        Debug.Log($"[ReadyManager] {PhotonNetwork.LocalPlayer.NickName}의 PlayerSlot 설정 완료: {mySlot}");
        if (!playerCharacterIndex.ContainsKey(mySlot))
            playerCharacterIndex[mySlot] = 0; // 기본 캐릭터 인덱스

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
        if (!CanControl(mySlot))
            return;

        if (!playerCharacterIndex.ContainsKey(mySlot))
            playerCharacterIndex[mySlot] = 0;

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
        if (!CanControl(mySlot))
            return;

        if (!playerCharacterIndex.ContainsKey(mySlot))
            playerCharacterIndex[mySlot] = 0;

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
        if (!CanControl(slot))
            return;

        playerReadyState[slot] = true;
        photonView.RPC("RPC_SetReady", RpcTarget.AllBuffered, slot, true);
    }

    public void OnCancel(int slot)
    {
        if (!CanControl(slot))
            return;

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
        for (int slot = 0; slot < 4; slot++) // 각 슬롯 UI 갱신
        {
            if (!playerCharacterIndex.ContainsKey(slot))
                playerCharacterIndex[slot] = 0;

            // 빈 슬롯 여부 판단: 각 플레이어의 CustomProperties["PlayerSlot"] 값으로 확인
            bool isSlotEmpty = !PhotonNetwork.PlayerList.Any(p => p.CustomProperties.ContainsKey("PlayerSlot") &&
                                                   (int)p.CustomProperties["PlayerSlot"] == slot);
            blackCharacters[slot].SetActive(isSlotEmpty);

            if (currentCharacters.ContainsKey(slot) && currentCharacters[slot] != null)
                currentCharacters[slot].SetActive(false);

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
        photonView.RPC("RPC_ChangeControl", RpcTarget.AllBuffered, currentPlayerSlot);
    }

    private bool CanControl(int slot)
    {
        return slot == (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"];
    }

    [PunRPC]
    private void RPC_ChangeControl(int newSlot)
    {
        currentPlayerSlot = newSlot;
        UpdateCharacterDisplay();
    }
}