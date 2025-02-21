using UnityEngine;

namespace KJ.Player
{
    public class PlayerState : MonoBehaviour
    {
        private Inventory inventory;   // Inventory 참조 변수
        private int health = 100;      // 플레이어 체력
        private bool isDead = false;   // 플레이어가 죽었는지 여부

        public bool saveLife = false;   // 죽음 면제 활성화 여부

        void Start()
        {
            inventory = FindFirstObjectByType<Inventory>();
        }

        void Update()
        {
            // T 키를 누르면 체력 감소 테스트
            if (Input.GetKeyDown(KeyCode.T))
            {
                TakeDamage(100);
            }
        }

        /// <summary>
        /// 아이템 사용 시 호출되는 메서드 ( Inventory의 UseItem() 호출 )
        /// </summary>
        public void ItemUse()
        {
            if (inventory != null)
            {
                inventory.UseItem();
            }
        }

        /// <summary>
        /// 플레이어가 피해를 받을 때 호출되는 메서드
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return; // 이미 죽었다면 데미지 무시

            health -= damage;
            Debug.Log($"플레이어가 {damage} 피해를 입었습니다. 현재 체력: {health}");

            if (health <= 0)
            {
                if (saveLife)
                {
                    saveLife = false; // 한 번만 죽음을 면제
                    health = 1; // 최소 체력 유지
                    Debug.Log("죽음을 면제받았습니다!");
                }
                else
                {
                    Die();
                }
            }
        }

        /// <summary>
        /// 플레이어가 사망할 때 호출되는 메서드
        /// </summary>
        private void Die()
        {
            isDead = true;
            Debug.Log("플레이어가 사망했습니다.");
        }
    }
}
