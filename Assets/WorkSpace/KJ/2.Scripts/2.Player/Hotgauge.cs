using UnityEngine;

namespace KJ.Player
{
    public class Hotgauge : MonoBehaviour
    {
        public bool gaugePause = false;
        private float heatGauge = 0f; // 🔥 초기값 명확히 설정
        private float maxHeat = 100f;
        private float heatIncreaseRate = 10f;
        private float heatDecreaseRate = 5f;

        private bool hasFire = false;

        [Header("UI 설정")]
        [SerializeField] private KJ.UI.HotgaugeUIController hotUI; // 뜨거움 게이지 UI 컨트롤러 추가

        private void Start()
        {
            ResetHeatOnDeath(); // ✅ 게임 시작 시 게이지 초기화
        }

        private void Update()
        {
            if (gaugePause) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFire();
            }

            if (hasFire)
            {
                IncreaseHeat(Time.deltaTime * heatIncreaseRate);
            }
            else if (heatGauge > 0)
            {
                DecreaseHeat(Time.deltaTime * heatDecreaseRate);
            }
        }

        /// <summary>
        /// 불을 들고 있는 상태 토글
        /// </summary>
        private void ToggleFire()
        {
            hasFire = !hasFire;
            Debug.Log($"불 상태 변경: {(hasFire ? "불을 들고 있음" : "불 없음")}");
        }

        /// <summary>
        /// 뜨거움 게이지 증가
        /// </summary>
        private void IncreaseHeat(float amount)
        {
            if (!hasFire) return;

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat);

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI();
        }

        /// <summary>
        /// 뜨거움 게이지 감소
        /// </summary>
        private void DecreaseHeat(float amount)
        {
            heatGauge -= amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat);
            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI();
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
            heatGauge = 0; // ✅ 사망 후 게이지 0으로 초기화
            gaugePause = false;
            hasFire = false; // 불 제거

            Debug.Log("핫게이지 초기화됨, 불 제거됨.");
            UpdateHotUI();
        }

        /// <summary>
        /// 핫게이지 UI 업데이트
        /// </summary>
        private void UpdateHotUI()
        {
            if (hotUI != null)
            {
                hotUI.UpdateHotUI(heatGauge, maxHeat);
            }
        }
    }
}