using UnityEngine;

namespace YJ.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("--- Panel ---")]
        [SerializeField] private GameObject escUI;
        [SerializeField] private GameObject soundUI;
        [SerializeField] private GameObject quitUI;


        public static UIController Instance { get; private set; }

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
                    escUI.SetActive(!escUI.activeSelf); // 현재 상태의 반대로 설정
                }
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