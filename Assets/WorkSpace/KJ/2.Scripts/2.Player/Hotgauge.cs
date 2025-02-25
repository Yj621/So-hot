using UnityEngine;
using System.Collections;

namespace KJ.Player
{
    public class Hotgauge : MonoBehaviour
    {
        public bool gaugePause = false;
        private float heatGauge = 0f;
        private float maxHeat = 100f;
        private float heatIncreaseRate = 10f;
        private float heatDecreaseRate = 5f;
        private float reviveDelay = 5f;

        private bool hasFire = false;
        private bool isDead = false;

        private PlayerAnimationController animationController;
        private PlayerMovement playerMovement;
        private Rigidbody rb;

        [Header("UI 설정")]
        [SerializeField] private KJ.UI.HotgaugeUIController hotUI; // 뜨거움 게이지 UI 컨트롤러 추가

        private void Awake()
        {
            animationController = GetComponent<PlayerAnimationController>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            UpdateHotUI(); // UI 초기값 설정
        }

        private void Update()
        {
            if (isDead) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFire();
            }

            if (hasFire && !gaugePause)
            {
                IncreaseHeat(Time.deltaTime * heatIncreaseRate);
            }
            else if (!hasFire && heatGauge > 0)
            {
                DecreaseHeat(Time.deltaTime * heatDecreaseRate);
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

        private void ToggleFire()
        {
            hasFire = !hasFire;
            Debug.Log($"불 상태 변경: {(hasFire ? "불을 들고 있음" : "불 없음")}");
        }

        private void IncreaseHeat(float amount)
        {
            if (!hasFire || isDead) return;

            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat);

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI();

            if (heatGauge >= maxHeat)
            {
                Overheat();
            }
        }

        private void DecreaseHeat(float amount)
        {
            heatGauge -= amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat);
            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");
            UpdateHotUI();
        }

        private void Overheat()
        {
            gaugePause = true;
            Debug.Log("플레이어가 과열되었습니다.");
            Die();
        }

        private void Die()
        {
            isDead = true;
            Debug.Log("플레이어가 사망했습니다.");

            playerMovement.enabled = false;
            rb.isKinematic = true;

            animationController?.PlayDeathAnimation();
            StartCoroutine(Revive());
        }

        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveDelay);

            isDead = false;
            heatGauge = 0;
            gaugePause = false;
            hasFire = false;

            Debug.Log("플레이어가 부활했습니다! (불 없음)");

            playerMovement.enabled = true;
            rb.isKinematic = false;

            animationController?.PlayReviveAnimation();
            UpdateHotUI();
        }

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
            UpdateHotUI();
        }

        private void UpdateHotUI()
        {
            if (hotUI != null)
            {
                hotUI.UpdateHotUI(heatGauge, maxHeat);
            }
        }
    }
}
