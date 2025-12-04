using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "Game/RoundData")]
public class RoundData : ScriptableObject
{
    [Header("적 프리팹 목록")]
    public GameObject[] enemyPrefabs;

    [Header("적 생성 위치")]
    public Vector2[] enemyPositions;

    [Header("장애물 프리팹 목록")]
    public GameObject[] obstaclePrefabs;

    [Header("장애물 위치")]
    public Vector2[] obstaclePositions;

    [Header("벽 프리팹")]
    public GameObject[] wallPrefabs;

    [Header("벽 위치")]
    public Vector2[] wallPositions;

    // 🔥🔥🔥 추가: 라운드별 플레이어 시작 위치
    [Header("플레이어 시작 위치")]
    public Vector2 userStartPosition; // ⬅️ 이 필드가 RoundManager에서 필요했습니다.
}