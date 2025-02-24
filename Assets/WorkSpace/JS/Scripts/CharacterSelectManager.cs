using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class CharacterSelectManager : MonoBehaviourPunCallbacks
{
    public ReadyManager[] playerObjects; // 씬에 미리 배치된 플레이어 오브젝트 배열
    private static List<int> assignedSlots = new List<int>(); // 사용 중인 슬롯 추적

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            AssignPlayerSlot();
        }
    }

    private void AssignPlayerSlot()
    {
        int assignedSlot = -1;

        // 1P~4P 중 비어 있는 슬롯 찾기
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].playerSlot == -1) // 빈 슬롯이면 할당
            {
                assignedSlot = i;
                playerObjects[i].playerSlot = assignedSlot;
                assignedSlots.Add(assignedSlot); // 슬롯을 사용 중으로 표시
                break;
            }
        }

        if (assignedSlot != -1)
        {
            // customProperties에 저장 (멀티플레이 동기화)
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "PlayerSlot", assignedSlot }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            Debug.Log($"플레이어 {PhotonNetwork.LocalPlayer.NickName}가 {assignedSlot + 1}P로 배정됨.");
        }
        else
        {
            Debug.LogError("슬롯 할당 실패! 모든 슬롯이 사용 중입니다.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.CustomProperties.ContainsKey("PlayerSlot"))
        {
            int freedSlot = (int)otherPlayer.CustomProperties["PlayerSlot"];
            assignedSlots.Remove(freedSlot); // 슬롯을 다시 사용 가능하도록 해제
            playerObjects[freedSlot].playerSlot = -1; // 슬롯 정보 초기화

            Debug.Log($"플레이어 {otherPlayer.NickName}가 퇴장하여 {freedSlot + 1}P 슬롯이 비워짐.");
        }
    }
}
