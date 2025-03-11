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

        private PhotonView pv;
        private GameManager gm;
        private GameObject player;
        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            gm = GameManager.Instance;
            StartCoroutine(StartGame());
        }

        private IEnumerator Loading()
        {
            SetTag("loadScene", true);
            while (!AllhasTag("loadScene")) yield return null;

            //플레이어 관련 정보를 커스텀 프로퍼티에서 가져옴
            int playerNumber = (int)GetTag(PhotonNetwork.LocalPlayer, "Number");
            CharacterType characterType = (CharacterType)GetTag(PhotonNetwork.LocalPlayer, "Character");

            //해당 정보 저장
            playerSetting = new PlayerSetting(playerNumber, characterType);

            if (gm == null) Debug.Log("null이네");
            // 모두 씬에 있어야 생성할 수 있음, 에디터와 클라는 에디터가 마스터
            player = PhotonNetwork.Instantiate("Character/"+playerSetting.type.ToString(), gm.spawnPoints[playerSetting.playerNumber].position, Quaternion.identity);

            //게임 매니저에 해당 플레이어를 넘겨준다
            gm.player = player;
            gm.playerNumber = playerNumber;

            //나머지 파츠들을 합침
            //pv.RPC("AddParts", RpcTarget.AllViaServer, gm.player.GetComponent<PhotonView>().ViewID, playerSetting.type);

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
