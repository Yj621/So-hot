using Photon.Pun;
using UnityEngine;
using YJ.Network;

namespace YJ.UI
{
    public class ESCPanelController : MonoBehaviour
    {
        [Header("--- Panel ---")]
        [SerializeField] private GameObject escUI;
        [SerializeField] private GameObject soundUI;
        [SerializeField] private GameObject quitUI;
        [SerializeField] private GameObject startUI;


        public static ESCPanelController Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = true;
                if (escUI != null)
                {
                    escUI.SetActive(!escUI.activeSelf); // 현재 상태의 반대로 설정
                }
            }
        }

        public void OnClickStart()
        {
            if (PhotonNetwork.InLobby)
            {
                startUI.SetActive(true); // 로비에 있을 경우 startUI 활성화
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        }


        //Sound 버튼 관련 함수
        public void OnClickSound()
        {
            soundUI.SetActive(true);
        }

        public void OnClickExitSound()
        {
            soundUI.SetActive(false);
        }

        // Quit버튼 관련 함수
        public void OnClickQuit()
        {
            //종료 확인 ui 활성화
            quitUI.SetActive(true);
        }

        public void OnClickQuitConfirm()
        {
            Application.Quit();
        }
        public void OnClickQuitCancle()
        {
            quitUI.SetActive(false);
        }
    }
}