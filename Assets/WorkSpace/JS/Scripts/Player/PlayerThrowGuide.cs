using UnityEngine;
using System.Collections.Generic;
using YJ.UIManager;
using JS.PlayerMove;

public class PlayerThrowGuide : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int lineSegmentCount = 30;  // 궤적을 구성할 점 개수
    public float simulationTimeStep = 0.05f; // 시뮬레이션 간격

    public Transform throwOrigin;  // 불이 던져질 시작 위치
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.enabled = false;
    }

    public void DrawThrowGuide(Vector3 throwDirection, float throwForce)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = lineSegmentCount;

        List<Vector3> points = new List<Vector3>();
        Vector3 currentPosition = throwOrigin.position;
        Vector3 currentVelocity = throwDirection * throwForce; // 던지는 힘 적용

        for (int i = 0; i < lineSegmentCount; i++)
        {
            points.Add(currentPosition);
            currentVelocity += Physics.gravity * simulationTimeStep; // 중력 적용
            currentPosition += currentVelocity * simulationTimeStep; // 위치 업데이트
        }

        lineRenderer.SetPositions(points.ToArray());
        Debug.DrawRay(throwOrigin.position, throwDirection * 5f, Color.red);
    }

    public void OffThrowGuide()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }
}
