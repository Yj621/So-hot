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
                if (escUI != null)
                {
                    bool isActive = !escUI.activeSelf;
                    escUI.SetActive(isActive); // 패널 상태 반전

                    if (isActive) // 패널이 활성화되었을 때
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else // 패널이 비활성화되었을 때
                    {
                        CloseAllSubPanels();
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
        }

        private void CloseAllSubPanels()
        {
            if (soundUI.activeSelf) soundUI.SetActive(false);
            if (quitUI.activeSelf) quitUI.SetActive(false);
            if (startUI.activeSelf) startUI.SetActive(false);
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