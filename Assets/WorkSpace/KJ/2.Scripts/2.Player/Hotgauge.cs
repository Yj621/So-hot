using UnityEngine;

namespace KJ.Player
{
    public class Hotgauge : MonoBehaviour
    {
        private Inventory inventory;    // Inventory 참조 변수
        public bool gaugePause = false; // true면 게이지가 안 올라감, 기본적으로 false
        private float heatGauge = 0f;   // 현재 뜨거움 게이지 (0 ~ 100)
        private float maxHeat = 100f;   // 최대 뜨거움 게이지
        private float heatIncreaseRate = 5f; // 초당 증가율

        void Start()
        {
            inventory = FindFirstObjectByType<Inventory>();
        }

        void Update()
        {
            if (!gaugePause)
            {
                IncreaseHeat(Time.deltaTime * heatIncreaseRate);
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
                Debug.Log("아이템 사용: 뜨거움 게이지 감소!");
            }
        }

        /// <summary>
        /// 뜨거움 게이지 증가 메서드
        /// </summary>
        public void IncreaseHeat(float amount)
        {
            heatGauge += amount;
            heatGauge = Mathf.Clamp(heatGauge, 0, maxHeat); // 0~100 범위 유지

            Debug.Log($"현재 뜨거움 게이지: {heatGauge}");

            if (heatGauge >= maxHeat)
            {
                Overheat(); // 최대치 도달 시 과열 처리
            }
        }

        /// <summary>
        /// 게이지 최대치 도달 시 과열 처리
        /// </summary>
        private void Overheat()
        {
            Debug.Log("플레이어가 과열되었습니다!");
            gaugePause = true; // 과열 시 게이지 멈춤
        }

        /// <summary>
        /// 뜨거움을 식혀서 다시 움직일 수 있도록 함
        /// </summary>
        public void ResetHeat()
        {
            heatGauge = 0;
            gaugePause = false; // 다시 게이지 증가 가능
            Debug.Log("게이지 초기화됨, 다시 시작 가능!");
        }
    }
}
