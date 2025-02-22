using UnityEngine;

namespace KJ.Player
{
    public class PlayerState : MonoBehaviour
    {
        private bool isDead = false;   // 플레이어가 죽었는지 여부

        public bool saveLife = false;  // 죽음 면제 활성화 여부

        void Update()
        {
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
        }
    }
}
