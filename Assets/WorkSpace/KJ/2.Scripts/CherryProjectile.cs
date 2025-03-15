using UnityEngine;
using Photon.Pun;

public class CherryProjectile : MonoBehaviour
{
    private MapController mapController;
    public GameObject hitEffect;

    public void Setup(MapController controller, GameObject hitVFX)
    {
        mapController = controller;
        hitEffect = hitVFX;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 🍒 플레이어 피격 이펙트 생성
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }
        }
    }
}
