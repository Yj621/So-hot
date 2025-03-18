using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : MonoBehaviour
{
    // 카메라가 이동할 위치들
    public List<Transform> cameraPoints;
    // 카메라가 바라볼 타겟 위치들
    public List<Transform> lookAtTargets;
    // 변경 간격 (초)
    public float switchInterval = 2f;

    private int currentIndex = 0;

    public List<float> holdDurations;

    private void Awake()
    {
        if (VoiceManager.Instance != null)
        {
            Destroy(VoiceManager.Instance.gameObject);
        }
    }

    private void Start()
    {
        if (holdDurations == null || holdDurations.Count != cameraPoints.Count)
        {
            holdDurations = new List<float>();
            for (int i = 0; i < cameraPoints.Count; i++)
            {
                holdDurations.Add(switchInterval);
            }
        }

        StartCoroutine(SwitchCameraView());
    }

    private IEnumerator SwitchCameraView()
    {
        while (true)
        {
            // 현재 인덱스의 카메라 포인트로 위치 설정
            transform.position = cameraPoints[currentIndex].position;
            // 바라볼 대상까지의 방향을 구해 회전 설정
            Vector3 direction = lookAtTargets[currentIndex].position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            float holdTime = holdDurations[currentIndex];

            // 다음 포인트로 인덱스 변경 (리스트 끝이면 처음으로)
            currentIndex = (currentIndex + 1) % cameraPoints.Count;

            yield return new WaitForSeconds(holdTime);
        }
    }
}
