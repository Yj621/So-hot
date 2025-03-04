using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using YJ.UIManager;
using System.Collections;

namespace KJ.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovement movement;
        public PlayerState state;
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
            animationController = GetComponent<PlayerAnimationController>();
            photonView = GetComponent<PhotonView>();
            //GameManager.Instance.RegisterPlayer(this);
        }

        void Start()
        {
            StartCoroutine(FindInventoryWithDelay());
        }

        IEnumerator FindInventoryWithDelay()
        {
            yield return new WaitForSeconds(5f); // 0.5초 정도 기다리기 (네트워크 동기화 시간 확보)
            inventory = FindAnyObjectByType<Inventory>();

            if (inventory == null)
            {
                Debug.LogError("Inventory 객체를 찾지 못함!");
            }
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