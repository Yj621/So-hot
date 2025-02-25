using System.Collections;
using UnityEngine;

namespace KJ.Player
{
    public class PlayerState : MonoBehaviour
    {
        private bool isDead = false;
        public bool saveLife = false;
        private Animator animator;
        private PlayerMovement playerMovement;
        private Rigidbody rb;
        private Hotgauge hotgauge; // 핫게이지 참조
        [SerializeField] private float reviveDelay = 5f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
            hotgauge = GetComponent<Hotgauge>(); // 핫게이지 참조
        }

        private void Update()
        {
            if (isDead) return;

            if (Input.GetKeyDown(KeyCode.T))
            {
                InstantKill();
            }

            // 핫게이지 값 체크하여 과열 사망 처리
            if (hotgauge != null && hotgauge.IsOverheated())
            {
                Die();
            }
        }

        /// <summary>
        /// 즉사 기믹 테스트
        /// </summary>
        public void InstantKill()
        {
            if (isDead) return;

            if (saveLife)
            {
                saveLife = false;
                Debug.Log("즉사 기믹을 면제받았습니다!");
            }
            else
            {
                Die();
            }
        }

        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            Debug.Log("플레이어가 사망했습니다.");

            // 이동 불가 처리
            playerMovement.enabled = false;
            rb.isKinematic = true;

            // 사망 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            // 핫게이지 초기화 (무조건 적용)
            if (hotgauge != null)
            {
                hotgauge.ResetHeatOnDeath();
            }

            // 일정 시간 후 부활
            StartCoroutine(Revive());
        }

        /// <summary>
        /// 일정 시간 후 부활
        /// </summary>
        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveDelay);

            isDead = false;
            Debug.Log("플레이어가 부활했습니다!");

            // 이동 가능 처리
            playerMovement.enabled = true;
            rb.isKinematic = false;

            // 부활 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("Revive");
            }
        }
    }
}
