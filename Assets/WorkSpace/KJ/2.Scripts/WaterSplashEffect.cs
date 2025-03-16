using UnityEngine;

public class WaterSplashEffect : MonoBehaviour
{
    [SerializeField] private GameObject splashEffectPrefab; // 물 튀는 이펙트 프리팹

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            // 이펙트 실행
            PlaySplashEffect(other.transform.position);
            SoundManager.Instance.PlaySound(SoundManager.AudioType.Water);
        }
    }

    private void PlaySplashEffect(Vector3 position)
    {
        // 물 표면에서 이펙트 생성
        if (splashEffectPrefab != null)
        {
            Vector3 effectPosition = new Vector3(position.x, transform.position.y, position.z); // 물 높이에 맞춤
            GameObject effect = Instantiate(splashEffectPrefab, effectPosition, Quaternion.identity);
            Destroy(effect, 2f); // 일정 시간 후 제거
        }
    }
}
