using KJ.CameraSystem;
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

        [SerializeField] private List<GameObject> playerTypes;
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>(4);

        private PhotonView pv;
        private GameObject player;
        private IPlayerBuider builder;


        public static SpawnManager Instance => instance;

        public static PlayerSetting PlayerSetting => playerSetting;

        private void Awake()
        {
            instance = this;
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            builder = new PlayerBuilder();
            builder.Character_Part(ref player, playerTypes[(int)playerSetting.type]);

            pv.RPC("SpawnPlayer", RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void SpawnPlayer()
        {
            player = Instantiate(player, spawnPoints[playerSetting.playerNumber].position, Quaternion.identity);
            //builder.Animator_Part(ref player);
            //builder.Effect_Part(ref player);
            builder.Skill_Part(ref player);

            if(pv.IsMine)
            {
                Camera.main.GetComponent<CameraController>().PlayerBody = player.transform;
            }
        }
    }
}
