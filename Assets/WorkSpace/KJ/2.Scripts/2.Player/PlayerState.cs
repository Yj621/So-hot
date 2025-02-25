using System.Collections;
using UnityEngine;

namespace KJ.Player
{
    public class PlayerState : MonoBehaviour
    {
        private bool isDead = false;   // 플레이어 사망 상태
        public bool saveLife = false;  // 한 번의 죽음을 면제받을 수 있는 상태
        private Animator animator;     // 애니메이터 참조
        private PlayerMovement playerMovement; // 플레이어 이동 컴포넌트
        private Rigidbody rb; // 물리적 이동을 제어하는 리지드바디
        private Hotgauge hotgauge; // 뜨거움 게이지 시스템
        [SerializeField] private float reviveDelay = 5f; // 부활까지의 대기 시간

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
            hotgauge = GetComponent<Hotgauge>();
        }

        private void Update()
        {
            if (isDead) return; // 사망 상태라면 업데이트 중단

            // 테스트용: T 키를 누르면 즉사
            if (Input.GetKeyDown(KeyCode.T))
            {
                InstantKill();
            }

            // 뜨거움 게이지가 최대치일 경우 사망 처리
            if (hotgauge != null && hotgauge.IsOverheated())
            {
                Die();
            }
        }

        /// <summary>
        /// 즉사 기믹 테스트 (디버그 용도)
        /// </summary>
        public void InstantKill()
        {
            if (isDead) return; // 이미 사망한 상태면 실행하지 않음

            if (saveLife)
            {
                saveLife = false; // 한 번의 죽음을 면제
                Debug.Log("즉사 기믹을 면제받았습니다!");
            }
            else
            {
                Die();
            }
        }

        /// <summary>
        /// 플레이어가 사망할 때 호출되는 메서드
        /// </summary>
        private void Die()
        {
            isDead = true;
            Debug.Log("플레이어가 즉사했습니다.");

            // 이동 불가 처리
            playerMovement.enabled = false;
            rb.isKinematic = true; // 물리적 이동 방지

            // 사망 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            // 뜨거움 게이지 초기화
            if (hotgauge != null)
            {
                hotgauge.ResetHeatOnDeath();
            }

            // 일정 시간 후 부활
            StartCoroutine(Revive());
        }

        /// <summary>
        /// 일정 시간 후 부활하는 코루틴
        /// </summary>
        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveDelay); // 부활 대기 시간

            isDead = false;
            Debug.Log("플레이어가 부활했습니다!");

            // 이동 가능하도록 복구
            playerMovement.enabled = true;
            rb.isKinematic = false;

            // 스태미나 최대치 회복
            if (playerMovement != null)
            {
                playerMovement.RecoverFullStamina();
            }

            // 부활 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("Revive");
            }
        }
    }
}
