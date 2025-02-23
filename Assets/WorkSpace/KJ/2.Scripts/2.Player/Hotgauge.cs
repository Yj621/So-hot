using UnityEngine;

namespace KJ.Player
{
    public class Hotgauge : MonoBehaviour
    {
        public bool gaugePause = false;
        private float heatGauge = 0f;
        private float maxHeat = 100f;
        private float heatIncreaseRate = 10f;

        private bool hasFire = false; // 불을 들고 있는지 여부 (테스트용)
        private bool isDead = false; // 플레이어 사망 여부

        private PlayerAnimationController animationController;

        private void Awake()
        {
            animationController = GetComponent<PlayerAnimationController>();
        }

        private void Update()
        {
            if (isDead) return; // 사망한 경우 입력을 처리하지 않음

            // F 키를 누르면 불을 들고 있는 상태 토글 (테스트용)
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFire();
            }

            if (hasFire && !gaugePause)
            {
                IncreaseHeat(Time.deltaTime * heatIncreaseRate);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                IncreaseHeat(5f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetHeat();
            }
        }

        /// <summary>
        /// 불을 들고 있는 상태를 토글 (테스트용)
        /// </summary>
        private void ToggleFire()
        {
            hasFire = !hasFire;
            Debug.Log($"불 상태 변경: {(hasFire ? "불을 들고 있음" : "불 없음")}");
        }

        /// <summary>
        /// 뜨거움 게이지 증가 (불을 들고 있을 때만)
        /// </summary>
        private void IncreaseHeat(float amount)
        {
            if (!hasFire || isDead) return; // 불이 없거나 이미 죽은 경우 증가 X

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat);

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");

            if (heatGauge >= maxHeat)
            {
                Overheat();
            }
        }

        /// <summary>
        /// 과열 처리 (게이지 최대치 도달)
        /// </summary>
        private void Overheat()
        {
            gaugePause = true;
            Debug.Log("플레이어가 과열되었습니다.");
            Die(); // 과열 시 사망 처리
        }

        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        private void Die()
        {
            isDead = true;
            Debug.Log("플레이어가 사망했습니다.");

            // 사망 애니메이션 실행
            animationController?.PlayDeathAnimation();
        }

        /// <summary>
        /// 뜨거움 게이지 초기화 (사망한 경우 리스폰 기능 추가 가능)
        /// </summary>
        private void ResetHeat()
        {
            if (isDead)
            {
                Debug.Log("사망한 상태에서는 게이지를 초기화할 수 없습니다.");
                return;
            }

            heatGauge = 0;
            gaugePause = false;
            Debug.Log("게이지 초기화됨, 다시 시작 가능.");
        }
    }
}
