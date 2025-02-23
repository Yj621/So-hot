using UnityEngine;

namespace KJ.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayerMovement playerMovement;
        private bool isDead = false; // 사망 상태 확인용

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (isDead) return; // 사망 상태에서는 애니메이션 업데이트 중단

            // 이동 속도를 애니메이션 블렌드 트리로 전달
            float speed = playerMovement.GetCurrentSpeedNormalized();
            animator.SetFloat("Speed", speed);

            // 점프 상태 확인하여 애니메이션 반영
            animator.SetBool("isGrounded", playerMovement.IsGrounded);
        }

        /// <summary>
        /// 플레이어 사망 애니메이션 실행
        /// </summary>
        public void PlayDeathAnimation()
        {
            if (animator != null && !isDead)
            {
                isDead = true;
                animator.SetTrigger("Die");
                Debug.Log("사망 애니메이션 실행");
            }
        }

        /// <summary>
        /// 플레이어 부활 애니메이션 실행
        /// </summary>
        public void PlayReviveAnimation()
        {
            if (animator != null && isDead)
            {
                isDead = false;
                animator.ResetTrigger("Die"); // 사망 트리거 해제
                animator.SetTrigger("Revive"); // 부활 트리거 실행
                Debug.Log("부활 애니메이션 실행");
            }
        }
    }
}
