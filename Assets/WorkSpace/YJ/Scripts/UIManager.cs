using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

namespace YJ.UIManager
{
    public class UIManager : MonoBehaviour
    {
        [Header("뜨거움 게이지 관련")]
        public bool gaugePause = false; // 게이지 일시 정지 상태 (true이면 증가/감소 멈춤)
        public float heatGauge = 0f; // 현재 뜨거움 게이지 값
        public float maxHeat = 100f; // 최대 뜨거움 게이지 값
        public float heatIncreaseRate = 10f; // 불을 들고 있을 때 초당 증가량
        public float heatDecreaseRate = 5f;  // 불이 없을 때 초당 감소량

        [Header("Slider 설정")]
        [SerializeField] private Slider hotSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Slider ThrowSlider;

        [Header("스태미나 관련")]
        public bool runLimit; // 스태미나 제한 여부 설정
        public float maxStamina = 100f; // 최대 스태미나 값
        public float currentStamina; // 현재 스태미나 값
        public float staminaDrainRate = 10f; // 스태미나 소모율


        public float currentThrow= 0f;
        public float maxThrow = 100f;
        public float ThrowIncreaseGauge;

       

        public static UIManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        void Start()
        {
            ResetHeatOnDeath(); // 게임 시작 시 게이지 초기화

            currentStamina = maxStamina;
        }


        // 실제 뜨거움 게이지 업데이트 함수
        public void UpdateHotGauge(float currentHeat, float maxHeat)
        {
            hotSlider.value = currentHeat / maxHeat;
        }

        // 실제 스태미나 게이지 업데이트 함수
        public void UpdateStaminaGauge(float currentStamina, float maxStamina)
        {
            staminaSlider.value = currentStamina / maxStamina;
        }

        public void UpdateThrowGauge(float currentThrow, float maxThrow)
        {
            ThrowSlider.value = currentThrow / maxThrow;
        }


        void Update()
        {

        }
        /// <summary>
        /// 뜨거움 게이지 증가 (최대값을 초과하지 않도록 제한)
        /// </summary>
        public void IncreaseHeat(float amount)
        {
            //if (!playerState.hasFire) return; // 불을 들고 있지 않으면 증가하지 않음

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최대치를 넘지 않도록 제한

            UpdateHotUI(); // UI 갱신
        }

        /// <summary>
        /// 뜨거움 게이지 감소 (최소값을 아래로 내려가지 않도록 제한)
        /// </summary>
        public void DecreaseHeat(float amount)
        {
            heatGauge -= amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최소 0 이하로 내려가지 않도록 제한

            UpdateHotUI(); // UI 갱신
        }

        /// <summary>
        /// 뜨거움 게이지가 최대치인지 확인
        /// </summary>
        public bool IsOverheated()
        {
            return heatGauge >= maxHeat;
        }

        /// <summary>
        /// 사망 후 뜨거움 게이지 초기화 및 불 제거
        /// </summary>
        public void ResetHeatOnDeath()
        {
            heatGauge = 0; // 게이지 0으로 초기화
            gaugePause = false; // 게이지 일시 정지 해제
            UpdateHotUI(); // UI 갱신
        }

        /// <summary>
        /// 핫게이지 UI 업데이트
        /// </summary>
        public void UpdateHotUI()
        {
            UpdateHotGauge(heatGauge, maxHeat); // 현재 게이지 상태를 UI에 반영

        }

        //스태미나 활성화
        public void ActiveStamina()
        {
            staminaSlider.gameObject.SetActive(true);
        }
        //스태미나 활성화
        public void DeactiveStamina()
        {
            staminaSlider.gameObject.SetActive(false);
        }

        /// <summary>
        /// 스태미나 감소
        /// </summary>
        public void DrainStamina()
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            UpdateStaminaUI(); // UI 업데이트
        }

        /// <summary>
        /// 스태미나 회복
        /// </summary>
        public void RecoverStamina()
        {
            if(currentStamina < maxStamina)
            {
                ActiveStamina();
                currentStamina += staminaDrainRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0 , maxStamina);
            }
            else
            {
                DeactiveStamina();
            }

            UpdateStaminaUI(); // UI 업데이트
        }
        /// <summary>
        /// 스태미나 UI 업데이트
        /// </summary>
        public void UpdateStaminaUI()
        {
            UpdateStaminaGauge(currentStamina, maxStamina);
        }


        public void RecoverFullStamina()
        {
            currentStamina = maxStamina;
            UpdateStaminaUI(); // UI 업데이트
        }

        public void ActiveThrow()
        {
            ThrowSlider.gameObject.SetActive(true);
        }
        //스태미나 활성화
        public void DeactiveThrow()
        {
            ThrowSlider.gameObject.SetActive(false);
        }


        public void UpdateThrowUI()
        {
            UpdateThrowGauge(currentThrow, maxThrow);
        }

        public void IncreaseCharge()
        {
            if (currentThrow < maxThrow)
            {
                ActiveThrow();
                currentThrow += ThrowIncreaseGauge * Time.deltaTime;
                currentThrow = Mathf.Clamp(currentThrow, 0, maxThrow);
            }
            UpdateThrowUI(); // UI 업데이트
        }

        public void ResetThrow()
        {
            currentThrow = 0;
            UpdateThrowUI(); // UI 업데이트
            DeactiveThrow();
        }

    }
}

