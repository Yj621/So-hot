using System.Collections;
using TMPro;
using UnityEngine;

namespace YJ.Timer
{
    public class DeadTimer : MonoBehaviour
    {
        [SerializeField] private float initialTime = 15.0f; // 초기 시간
        private float time;
        public TextMeshProUGUI timerText;

        void Start()
        {

        }

        private void OnEnable()
        {
            ResetTimer();
        }

        private void ResetTimer()
        {
            StopAllCoroutines(); // 현재 실행 중인 코루틴 중지
            time = initialTime; // 시간을 초기값으로 설정
            timerText.color = new Color32(0x5A, 0x58, 0x55, 0xFF);
            StartTimer(); // 타이머 다시 시작
        }

        private void StartTimer()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            while (time > 0)
            {
                int minutes = Mathf.FloorToInt(time / 60); // 분 계산
                int seconds = Mathf.FloorToInt(time % 60); // 초 계산
                timerText.text = $"{minutes:D2}:{seconds:D2}"; // 두 자리 분:초 형식

                yield return new WaitForSeconds(1.0f);
                time -= 1;
                if (time < 5)
                {
                    timerText.color = Color.red;
                }
            }
            timerText.text = "00:00";
            TimerEnd();
        }

        private void TimerEnd()
        {
            //부활시 Die Timer 비활성화
            Debug.Log("타이머 끝, 부활");
        }
    }


}