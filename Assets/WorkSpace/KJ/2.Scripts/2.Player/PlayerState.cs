using System.Collections;
using UnityEngine;

namespace KJ.Player
{
    public class PlayerState : MonoBehaviour
    {
        public bool isDead = false;   // 플레이어가 죽었는지 여부
        public bool saveLife = false;  // 죽음 면제 활성화 여부
        private Animator animator;     // 애니메이터 참조
        private PlayerMovement playerMovement; // 플레이어 이동 컴포넌트
        private Rigidbody rb; // 리지드바디 참조
        [SerializeField] private float reviveDelay = 5f; // 부활 대기 시간

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isDead) return; // 사망 상태에서는 입력을 받지 않음

            // 테스트: T 키를 누르면 즉사
            if (Input.GetKeyDown(KeyCode.T))
            {
                InstantKill();
            }
        }

        /// <summary>
        /// 즉사 기믹 테스트
        /// </summary>
        public void InstantKill()
        {
            if (isDead) return; // 이미 죽었다면 무시

            if (saveLife)
            {
                saveLife = false; // 한 번은 면제 가능
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

            // 일정 시간 후 부활
            StartCoroutine(Revive());
        }

        /// <summary>
        /// 일정 시간 후 부활하는 코루틴
        /// </summary>
        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveDelay); // 부활 대기

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
