using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

        [Header("타이머 관련")]
        [SerializeField] private float initialTime = 15.0f; // 초기 시간
        [SerializeField] private GameObject dieTimerPanel;
        private float time;
        public TextMeshProUGUI timerText;

        [Header("던지는 게이지 관련")]
        public float currentThrow = 0f;
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


        //뜨거움 게이지 관련 함수

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


        //뜨거움 게이지 증가 (최대값을 초과하지 않도록 제한)
        public void IncreaseHeat(float amount)
        {
            if (gaugePause) return;
            //if (!playerState.hasFire) return; // 불을 들고 있지 않으면 증가하지 않음

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최대치를 넘지 않도록 제한

            //Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI(); // UI 갱신
        }

        //뜨거움 게이지 감소 (최소값을 아래로 내려가지 않도록 제한)
        public void DecreaseHeat(float amount)
        {
            heatGauge -= amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최소 0 이하로 내려가지 않도록 제한

            //Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI(); // UI 갱신
        }

        //뜨거움 게이지가 최대치인지 확인
        public bool IsOverheated()
        {
            return heatGauge >= maxHeat;
        }

        //사망 후 뜨거움 게이지 초기화 및 불 제거
        public void ResetHeatOnDeath()
        {
            heatGauge = 0; // 게이지 0으로 초기화
            gaugePause = false; // 게이지 일시 정지 해제

            Debug.Log("핫게이지 초기화됨, 불 제거됨.");
            UpdateHotUI(); // UI 갱신
        }

        //핫게이지 UI 업데이트
        public void UpdateHotUI()
        {
            UpdateHotGauge(heatGauge, maxHeat); // 현재 게이지 상태를 UI에 반영

        }

        //스태미나 관련 함수

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

        //스태미나 감소
        public void DrainStamina()
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            UpdateStaminaUI(); // UI 업데이트
        }

        //스태미나 회복
        public void RecoverStamina()
        {
            if (currentStamina < maxStamina)
            {
                ActiveStamina();
                currentStamina += staminaDrainRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                //Debug.Log("회복중,,");
            }
            else
            {
                DeactiveStamina();
            }

            UpdateStaminaUI(); // UI 업데이트
        }

        //스태미나 UI 업데이트
        public void UpdateStaminaUI()
        {
            UpdateStaminaGauge(currentStamina, maxStamina);
        }


        public void RecoverFullStamina()
        {
            currentStamina = maxStamina;
            UpdateStaminaUI(); // UI 업데이트
        }


        //던지는 게이지 관련 함수

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
                //Debug.Log("던짐 게이지 차는중");
            }
            UpdateThrowUI(); // UI 업데이트
        }

        public void ResetThrow()
        {
            currentThrow = 0;
            UpdateThrowUI(); // UI 업데이트
            DeactiveThrow();
        }


        //타이머 관련 함수      

        // 타이머 ON  
        public void TimerStart()
        {
            dieTimerPanel.SetActive(true);
            ResetTimer();
        }
        private void ResetTimer()
        {
            StopAllCoroutines(); // 현재 실행 중인 코루틴 중지
            time = initialTime; // 시간을 초기값으로 설정
            timerText.color = new Color32(0x5A, 0x58, 0x55, 0xFF);
            StartTimer(); // 타이머 다시 시작
        }

        private void StartTimer()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            while (time > 0)
            {
                int minutes = Mathf.FloorToInt(time / 60); // 분 계산
                int seconds = Mathf.FloorToInt(time % 60); // 초 계산
                timerText.text = $"{minutes:D2}:{seconds:D2}"; // 두 자리 분:초 형식
                time -= 1;
                yield return new WaitForSeconds(1.0f);
                if (time < 5)
                {
                    timerText.color = Color.red;
                }
            }
            timerText.text = "00:00";
            TimerEnd();
        }

        public void TimerEnd()
        {
            //부활시 Die Timer 비활성화
            dieTimerPanel.SetActive(false);
            Debug.Log("타이머 끝, 부활");
        }
    }
    
}

