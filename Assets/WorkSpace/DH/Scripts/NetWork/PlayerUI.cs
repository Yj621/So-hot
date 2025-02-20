using Photon.Pun;
using UnityEngine;
using TMPro;

namespace Donghyun.Network
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject readyText;
        [SerializeField] private GameObject masterText;
        [SerializeField] private TextMeshProUGUI nameText;


        public void SetClient() 
        {
            readyText.SetActive(false);
            masterText.SetActive(false);
        }

        public void SetMaster()
        {
            readyText.SetActive(false);
            masterText.SetActive(true);
        }

        public void SetReady(bool isReady)
        {
            readyText.SetActive(isReady);
        }

        /// <summary>
        /// 플레이어의 이름 UI를 업데이트
        /// </summary>
        /// <param name="name">플레이어 이름</param>
        public void SetNickname(string name)
        {
            nameText.text = name;
        }
    }

}

