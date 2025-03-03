using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Donghyun.Builder
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> spanwPoints = new List<Transform>(4);

        private PhotonView pv;
        private GameObject player;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            IPlayerBuider builder = new PlayerBuilder();
            builder.Animator_Part(PlayerState.Blue);
            builder.State_Part(PlayerState.Blue);
            player = builder.Result();

            pv.RPC("SpanwPlayer", RpcTarget.AllViaServer);
        }

        private void SpawnPlayer()
        {
            //Instantiate(player, spawnPoints[]);
        }
    }
}
