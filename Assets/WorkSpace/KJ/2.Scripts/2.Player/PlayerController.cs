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
        public Coroutine unlimitRunCoroutine; 
        public Coroutine gaugeStopCoroutine;

        private Inventory inventory;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            state = GetComponent<PlayerState>();
            hotgauge = GetComponent<Hotgauge>();
            animationController = GetComponent<PlayerAnimationController>();
            photonView = GetComponent<PhotonView>();
        }

        void Start()
        {
            inventory = FindAnyObjectByType<Inventory>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ItemUse();
            }
        }

        public void ItemUse()
        {
            if (inventory != null)
            {
                inventory.UseItem();
            }
        }
    }
}