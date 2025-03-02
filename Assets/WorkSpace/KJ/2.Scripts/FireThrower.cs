using UnityEngine;

public class FireThrower : MonoBehaviour
{
    [SerializeField] private Transform handPosition; // 플레이어 손 위치
    [SerializeField] private float throwForce = 10f; // 던지는 힘

    private GameObject heldFire; // 현재 들고 있는 불 오브젝트
    private bool isHoldingFire = false; // 불을 들고 있는지 여부

    private void OnTriggerEnter(Collider other)
    {
        // 불 오브젝트에 "Fire" 태그가 붙어 있고, 아직 불을 들고 있지 않을 때
        if (other.CompareTag("Fire") && !isHoldingFire)
        {
            GrabFire(other.gameObject);
        }
    }

    private void Update()
    {
        // 불을 들고 있을 때 마우스 왼쪽 버튼을 누르면 던짐
        if (isHoldingFire && Input.GetMouseButtonDown(0))
        {
            ThrowFire();
        }
    }

    private void GrabFire(GameObject fire)
    {
        // 불을 잡음
        heldFire = fire;
        isHoldingFire = true;

        // Rigidbody 가져오기
        Rigidbody fireRb = heldFire.GetComponent<Rigidbody>();
        Collider fireCollider = heldFire.GetComponent<Collider>();

        if (fireRb != null)
        {
            fireRb.isKinematic = true; // 물리 비활성화 (중력 제거)
        }

        if (fireCollider != null)
        {
            fireCollider.isTrigger = true; // 트리거 유지 (잡을 때 감지)
        }

        // 불을 손 위치로 이동시키고 부모 설정
        heldFire.transform.position = handPosition.position;
        heldFire.transform.SetParent(handPosition);
    }

    private void ThrowFire()
    {
        // 손에서 불을 놓음
        heldFire.transform.SetParent(null);
        isHoldingFire = false;

        // Rigidbody 및 Collider 다시 활성화
        Rigidbody fireRb = heldFire.GetComponent<Rigidbody>();
        Collider fireCollider = heldFire.GetComponent<Collider>();

        if (fireRb != null)
        {
            fireRb.isKinematic = false; // 물리 활성화
            fireRb.linearVelocity = Vector3.zero; // 기존 속도 초기화
            fireRb.AddForce(transform.forward * throwForce, ForceMode.Impulse); // 앞으로 던지기
        }

        if (fireCollider != null)
        {
            fireCollider.isTrigger = false; // 던질 때는 트리거 비활성화 (충돌 가능)
        }

        heldFire = null;
    }
}
