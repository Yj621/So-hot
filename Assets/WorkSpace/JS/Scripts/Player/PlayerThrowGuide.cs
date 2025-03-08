using UnityEngine;
using System.Collections.Generic;

public class PlayerThrowGuide : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int lineSegmentCount = 30;  // 궤적을 구성할 점 개수
    public float simulationTimeStep = 0.05f; // 시뮬레이션 간격

    public Transform throwOrigin;  // 불이 던져질 시작 위치
   
    private Camera mainCamera;
    private PlayerMove playerMove;

    private void Start()
    {
        mainCamera = Camera.main;
        playerMove = GetComponent<PlayerMove>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        Debug.DrawRay(throwOrigin.position, Camera.main.transform.forward * 2, Color.red, 2f);
        if (playerMove.isThrowingReady)
            DrawThrowGuide();

        if(!playerMove.isThrowingReady)
            OffThrowGuide();
    }

    void DrawThrowGuide()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = lineSegmentCount;

        // 카메라 방향을 가져와 던질 방향 계산
        Vector3 throwDirection = mainCamera.transform.forward;

        // 현재 충전된 던지기 힘
        float throwForce = Mathf.Lerp(playerMove.minThrowForce, playerMove.maxThrowForce, playerMove.throwChargeTime / playerMove.maxChargeTime);

        // 중력을 고려한 궤적 계산
        List<Vector3> points = new List<Vector3>();
        Vector3 currentPosition = throwOrigin.position;
        Vector3 currentVelocity = throwDirection * throwForce;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            points.Add(currentPosition);
            currentVelocity += Physics.gravity * simulationTimeStep; // 중력 적용
            currentPosition += currentVelocity * simulationTimeStep; // 위치 업데이트
        }

        lineRenderer.SetPositions(points.ToArray());
    }

    void OffThrowGuide()
    { 
       lineRenderer.enabled = false;
    }
}
