using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TotalMultiManager;

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
            StartCoroutine(StartGame());
        }

        private IEnumerator Loading()
        {
            SetTag("loadScene", true);
            while (!AllhasTag("loadScene")) yield return null;

            player = new GameObject();
            int playerNumber = (int)GetTag(PhotonNetwork.LocalPlayer, "Number");
            CharacterType characterType = (CharacterType)GetTag(PhotonNetwork.LocalPlayer, "Character");

            playerSetting = new PlayerSetting(playerNumber, characterType);
            pv = GetComponent<PhotonView>();

            // 모두 씬에 있어야 생성할 수 있음, 에디터와 클라는 에디터가 마스터
            player = PhotonNetwork.Instantiate(playerSetting.type.ToString(), spawnPoints[playerSetting.playerNumber].position, Quaternion.identity);

            pv.RPC("AddParts", RpcTarget.AllViaServer, player.GetComponent<PhotonView>().ViewID, playerSetting.type);

            while (AllhasTag("loadPlayer")) yield return null;
        }

        private IEnumerator StartGame()
        {
            yield return Loading();

            if (master())
            {
                
            }
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
