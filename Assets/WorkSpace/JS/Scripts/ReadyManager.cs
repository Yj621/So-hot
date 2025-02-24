using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

public class ReadyManager : MonoBehaviourPunCallbacks
{
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
        if (PhotonNetwork.IsMasterClient)
        {
            AssignPlayerSlot();
        }

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

    private void AssignPlayerSlot()
    {
        List<int> usedSlots = new List<int>();

        // 이미 사용 중인 슬롯 찾기
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("PlayerSlot"))
            {
                usedSlots.Add((int)player.CustomProperties["PlayerSlot"]);
            }
        }

        // 비어있는 슬롯 찾기
        int assignedSlot = -1;
        for (int i = 0; i < 4; i++)
        {
            if (!usedSlots.Contains(i))
            {
                assignedSlot = i;
                break;
            }
        }

        if (assignedSlot != -1)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerSlot", assignedSlot } });

            playerCharacterIndex[assignedSlot] = 0;
            playerReadyState[assignedSlot] = false;
        }
    }


    private void SetupUI()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerSlot"))
        {
            mySlot = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"];
            Debug.Log($"[ReadyManager] {PhotonNetwork.LocalPlayer.NickName}이(가) {mySlot + 1}P로 설정됨.");
            blackCharacters[mySlot].SetActive(true);
        }

        UpdateCharacterDisplay();
    }

    private void BindButtonEvents()
    {
        if (leftArrowButton != null)
        {
            leftArrowButton.onClick.AddListener(() => OnLeftArrow());
            Debug.Log("[ReadyManager] 왼쪽 화살표 버튼 이벤트 등록됨.");
        }

        if (rightArrowButton != null)
        {
            rightArrowButton.onClick.AddListener(() => OnRightArrow());
            Debug.Log("[ReadyManager] 오른쪽 화살표 버튼 이벤트 등록됨.");
        }

        if (readyButton != null)
        {
            readyButton.onClick.AddListener(() => OnSelect(mySlot));
            Debug.Log("[ReadyManager] 레디 버튼 이벤트 등록됨.");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(() => OnCancel(mySlot));
            Debug.Log("[ReadyManager] 취소 버튼 이벤트 등록됨.");
        }
    }


    public void OnLeftArrow()
    {
        if (!CanControl(mySlot)) return;

        if (!playerCharacterIndex.ContainsKey(mySlot))
        {
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
        for (int slot = 0; slot < 4; slot++) // 모든 플레이어의 캐릭터 UI 갱신
        {
            if (!playerCharacterIndex.ContainsKey(slot))
            {
                playerCharacterIndex[slot] = -1;
            }

            // 슬롯이 비어 있으면 ??? 표시
            bool isSlotEmpty = !PhotonNetwork.PlayerList.Any(p => p.CustomProperties.ContainsKey("PlayerSlot") && (int)p.CustomProperties["PlayerSlot"] == slot);
            blackCharacters[slot].SetActive(isSlotEmpty);

            // 기존 캐릭터 비활성화
            if (currentCharacters.ContainsKey(slot) && currentCharacters[slot] != null)
            {
                currentCharacters[slot].SetActive(false);
            }

            // 새로운 캐릭터 표시
            GameObject selectedCharacter = null;
            if (!isSlotEmpty && playerCharacterIndex[slot] != -1)
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

            // UI 업데이트
            characterDisplay.sprite = (playerCharacterIndex[mySlot] == -1)
                ? unknownCharacterSprite
                : characterImages[playerCharacterIndex[mySlot]];
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
        return PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerSlot") &&
               (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"] == slot;
    }

    [PunRPC]
    private void RPC_ChangeControl(int newSlot)
    {
        currentPlayerSlot = newSlot;
        UpdateCharacterDisplay();
    }
}
