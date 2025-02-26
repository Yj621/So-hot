using UnityEngine;

namespace KJ.Player
{
    public class Hotgauge : MonoBehaviour
    {
        public bool gaugePause = false; // 게이지 일시 정지 상태 (true이면 증가/감소 멈춤)
        private float heatGauge = 0f; // 현재 뜨거움 게이지 값
        private float maxHeat = 100f; // 최대 뜨거움 게이지 값
        private float heatIncreaseRate = 10f; // 불을 들고 있을 때 초당 증가량
        private float heatDecreaseRate = 5f;  // 불이 없을 때 초당 감소량

        private bool hasFire = false; // 플레이어가 불을 들고 있는 상태 여부

        [Header("UI 설정")]
        [SerializeField] private KJ.UI.HotgaugeUIController hotUI; // 뜨거움 게이지 UI 컨트롤러

        private void Start()
        {
            ResetHeatOnDeath(); // 게임 시작 시 게이지 초기화
        }

        private void Update()
        {
            if (gaugePause) return; // 게이지 일시 정지 상태라면 업데이트 중단

            if (Input.GetKeyDown(KeyCode.F)) // F 키를 눌러 불을 켜거나 끌 수 있음
            {
                ToggleFire();
            }

            if (hasFire)
            {
                IncreaseHeat(Time.deltaTime * heatIncreaseRate); // 불을 들고 있으면 뜨거움 게이지 증가
            }
            else if (heatGauge > 0)
            {
                DecreaseHeat(Time.deltaTime * heatDecreaseRate); // 불이 없으면 뜨거움 게이지 감소
            }
        }

        /// <summary>
        /// 불을 들고 있는 상태를 토글 (켜기/끄기)
        /// </summary>
        private void ToggleFire()
        {
            hasFire = !hasFire; // 현재 불 상태 반전
            Debug.Log($"불 상태 변경: {(hasFire ? "불을 들고 있음" : "불 없음")}");
        }

        /// <summary>
        /// 뜨거움 게이지 증가 (최대값을 초과하지 않도록 제한)
        /// </summary>
        private void IncreaseHeat(float amount)
        {
            if (!hasFire) return; // 불을 들고 있지 않으면 증가하지 않음

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최대치를 넘지 않도록 제한

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI(); // UI 갱신
        }

        /// <summary>
        /// 뜨거움 게이지 감소 (최소값을 아래로 내려가지 않도록 제한)
        /// </summary>
        private void DecreaseHeat(float amount)
        {
            heatGauge -= amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 최소 0 이하로 내려가지 않도록 제한

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
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
            hasFire = false; // 불 제거

            Debug.Log("핫게이지 초기화됨, 불 제거됨.");
            UpdateHotUI(); // UI 갱신
        }

        /// <summary>
        /// 핫게이지 UI 업데이트
        /// </summary>
        private void UpdateHotUI()
        {
            if (hotUI != null)
            {
                hotUI.UpdateHotUI(heatGauge, maxHeat); // 현재 게이지 상태를 UI에 반영
            }
        }
    }
}
