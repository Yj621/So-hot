using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Donghyun.Builder
{
    public class SpawnManager : MonoBehaviour
    {
        private static SpawnManager instance;
        private static PlayerSetting playerSetting = new PlayerSetting();

        [SerializeField] private List<Transform> spanwPoints = new List<Transform>(4);

        private PhotonView pv;
        private GameObject player;

        public static SpawnManager Instance => instance;

        public static PlayerSetting PlayerSetting => playerSetting;

        private void Awake()
        {
            instance = this;
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            IPlayerBuider builder = new PlayerBuilder();
            builder.Character_Part(playerSetting.type);
            builder.Animator_Part();
            builder.Effect_Part();
            builder.Skill_Part();
            player = builder.Result();

            pv.RPC("SpanwPlayer", RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void SpawnPlayer()
        {
            Instantiate(player, spanwPoints[playerSetting.playerNumber].position, Quaternion.identity);
        }
    }
}
