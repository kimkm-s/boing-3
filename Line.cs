using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    [Header("추적할 오브젝트")]
    public Transform target; // 궤적을 남길 대상

    [Header("점 추가 기준")]
    public float minDistance = 0.05f; // 이전 점과 최소 거리

    private LineRenderer lineRenderer;
    private List<Vector3> linePoints = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        if (target != null)
        {
            linePoints.Add(target.position);
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, target.position);
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 currentPos = target.position;
        if (linePoints.Count == 0 || Vector3.Distance(linePoints[linePoints.Count - 1], currentPos) >= minDistance)
        {
            linePoints.Add(currentPos);
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
        }
    }

    // 궤적 초기화 함수 (원할 때 호출)
    public void ClearTrajectory()
    {
        linePoints.Clear();
        lineRenderer.positionCount = 0;
    }
}
