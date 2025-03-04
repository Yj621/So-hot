using KJ.CameraSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Donghyun.Builder
{
    public class SpawnManager : MonoBehaviour
    {
        private PlayerSetting playerSetting;

        [SerializeField] private List<Transform> spawnPoints = new List<Transform>(4);

        private PhotonView pv;
        private GameObject player;

        private void Awake()
        {
            player = new GameObject();
            playerSetting = new PlayerSetting((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"], (CharacterType)PhotonNetwork.LocalPlayer.CustomProperties["Character"]);
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            player = PhotonNetwork.Instantiate(playerSetting.type.ToString(), spawnPoints[playerSetting.playerNumber].position, Quaternion.identity);

            pv.RPC("AddParts", RpcTarget.AllViaServer, player.GetComponent<PhotonView>().ViewID, playerSetting.type);
        }

        [PunRPC]
        private void AddParts(int viewID, CharacterType type)
        {
            PhotonView targetView = PhotonView.Find(viewID);

            GameObject go = targetView.gameObject;
            IPlayerBuider builder = GetComponent<IPlayerBuider>();
            //builder.Character_Part(playerSetting.type);
            builder.Effect_Part();
            builder.Skill_Part();
            List<GameObject> parts = builder.Return_Parts();

            foreach(GameObject part in parts)
            {
                Debug.Log(part);
                Instantiate(part, go.transform);
            }
        }
    }
}
