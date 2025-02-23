using UnityEngine;

namespace KJ.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayerMovement playerMovement;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
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
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
        }
    }
}
