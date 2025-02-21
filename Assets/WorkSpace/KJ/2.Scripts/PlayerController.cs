using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

namespace KJ.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovement movement;
        public PlayerState state;
        public Hotgauge hotgauge;
        public PlayerAnimationController animationController;
        public List<GameObject> effectList;
        public PhotonView photonView;
        public Coroutine unlimitRunCoroutine; // 실행 중인 unlimit run 코루틴 저장
        public Coroutine gaugeStopCoroutine; // 실행 중인 gauge stop 코루틴 저장

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            state = GetComponent<PlayerState>();
            hotgauge = GetComponent<Hotgauge>();
            animationController = GetComponent<PlayerAnimationController>();
            photonView = GetComponent<PhotonView>();

        }
    }
}