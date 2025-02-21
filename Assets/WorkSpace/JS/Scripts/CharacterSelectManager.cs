using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviourPunCallbacks
{
    public ReadyManager readyManager;

    private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>(); // 플레이어별 준비 상태 저장
    private Dictionary<int, int> playerCharacterSelections = new Dictionary<int, int>(); // 플레이어별 캐릭터 선택

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            AssignPlayerSlot();
        }
    }

    private void AssignPlayerSlot()
    {
        HashSet<int> usedSlots = new HashSet<int>();

        // 현재 방에 있는 플레이어들의 슬롯 정보를 수집
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.CustomProperties.ContainsKey("PlayerSlot"))
            {
                usedSlots.Add((int)player.CustomProperties["PlayerSlot"]);
            }
        }

        int assignedSlot = -1;

        // 비어있는 슬롯 찾기
        for (int i = 0; i < 4; i++)
        {
            if (!usedSlots.Contains(i)) // 사용되지 않은 슬롯이면 할당
            {
                assignedSlot = i;
                break;
            }
        }

        if (assignedSlot != -1)
        {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "PlayerSlot", assignedSlot }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            Debug.Log($"[LobbyManager] 플레이어 {PhotonNetwork.LocalPlayer.NickName}가 {assignedSlot + 1}P로 배정됨.");

            // ReadyManager UI 업데이트
            readyManager.UpdateSlotUI(assignedSlot, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.LogError("[LobbyManager] 슬롯 할당 실패! 모든 슬롯이 사용 중입니다.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.CustomProperties.ContainsKey("PlayerSlot"))
        {
            int freedSlot = (int)otherPlayer.CustomProperties["PlayerSlot"];

            // ReadyManager 슬롯 초기화
            //readyManager.ClearSlot(freedSlot);

            Debug.Log($"[LobbyManager] 플레이어 {otherPlayer.NickName}가 퇴장하여 {freedSlot + 1}P 슬롯이 비워짐.");
        }
    }

    public void SetReady(int slot, bool isReady)
    {
        playerReadyStatus[slot] = isReady;
        photonView.RPC("RPC_UpdateReadyStatus", RpcTarget.AllBuffered, slot, isReady);
        //networkManager.UpdateReadyStatus(); // NetWorkManager에게 Ready 상태 전달
    }

    public void SetCharacterSelection(int slot, int characterIndex)
    {
        playerCharacterSelections[slot] = characterIndex;
        photonView.RPC("RPC_UpdateCharacterSelection", RpcTarget.AllBuffered, slot, characterIndex);
    }

    [PunRPC]
    private void RPC_UpdateReadyStatus(int slot, bool isReady)
    {
        playerReadyStatus[slot] = isReady;
    }

    [PunRPC]
    private void RPC_UpdateCharacterSelection(int slot, int characterIndex)
    {
        playerCharacterSelections[slot] = characterIndex;
    }
}

