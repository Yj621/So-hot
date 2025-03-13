using Donghyun.Network;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using static TotalMultiManager;


namespace Donghyun.Network
{
    public class PlayerNetWorkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private PhotonView pv;

        private Hashtable ht; //커스텀 프로퍼티 캐싱
        private Slot emptyPlayer = new Slot(); //플레이어 슬롯

        private void Awake()
        {
            ht = PhotonNetwork.CurrentRoom.CustomProperties; //커스텀 프로퍼티 캐싱
        }

        //누군가 방을 떠날때
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("LobbyScene - 인원 퇴장");

            ConvertJsonToEmptyPlayerSlot();
            emptyPlayer.slot.Add((int)GetTag(otherPlayer, "Number"));
            ConvertEmptyPlayerSlotToJson();

            otherPlayer.CustomProperties.Clear();

            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        private void ConvertJsonToEmptyPlayerSlot()
        {
            object JSonData;
            ht.TryGetValue("EmptyPlayerSlot", out JSonData);

            SerializableSlot emptySlot = JsonUtility.FromJson<SerializableSlot>((string)JSonData);
            emptyPlayer.slot = new SortedSet<int>(emptySlot.slot);

            emptySlot.slot.Clear();
        }

        private void ConvertEmptyPlayerSlotToJson()
        {
            SerializableSlot emptySlot = new SerializableSlot();
            emptySlot.slot = new List<int>(emptyPlayer.slot);

            string ConvertJson = JsonUtility.ToJson(emptySlot);
            ht["EmptyPlayerSlot"] = ConvertJson;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            emptySlot.slot.Clear();
        }
    }

}