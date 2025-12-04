using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("라운드 데이터 리스트")]
    public RoundData[] roundDatas;

    private int currentRound = 0;

    private GameObject roundContainer;

    [Header("플레이어 설정")]
    public GameObject User; // public으로 선언해야 인스펙터에서 할당 가능
    private GameObject currentPlayerInstance; // 이 변수도 필드에 있어야 합니다.

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadRound(roundDatas[currentRound]);
    }
    public void LoadRound(RoundData data)
    {
        // 1. 이전 라운드 오브젝트 삭제
        if (roundContainer != null)
            Destroy(roundContainer);

        // 2. 이전 플레이어 인스턴스 삭제 (새 라운드마다 플레이어를 새로 생성하는 방식일 경우)
        if (currentPlayerInstance != null)
            Destroy(currentPlayerInstance);

        // 3. 새 라운드 컨테이너 생성
        roundContainer = new GameObject("RoundObjects");

        // 4. 오브젝트 배치 (몬스터, 벽 등)
        SpawnObjects(data.enemyPrefabs, data.enemyPositions);
        SpawnObjects(data.obstaclePrefabs, data.obstaclePositions);
        SpawnObjects(data.wallPrefabs, data.wallPositions);

        // 5. 🔥 플레이어 생성 및 위치 설정 (RoundData.userStartPosition 사용 가정)
        // RoundData 클래스에 public Vector2 userStartPosition; 변수가 있다고 가정합니다.
        Vector2 playerStartPosition = data.userStartPosition;

        if (User != null)
        {
            // Vector3로 위치를 생성
            currentPlayerInstance = Instantiate(User, playerStartPosition, Quaternion.identity);
        }

        // 6. 🔥 모든 MonsterAttack에게 새로 생성된 플레이어의 위치를 전달

        // 씬에 있는 모든 몬스터를 찾습니다. (이 시점에 몬스터가 모두 생성되었으므로 안전함)
        MonsterAttack[] monsters = FindObjectsOfType<MonsterAttack>();

        // 새로 생성된 플레이어의 Transform
        Transform playerTarget = currentPlayerInstance != null ? currentPlayerInstance.transform : null;

        if (playerTarget != null)
        {
            foreach (MonsterAttack monster in monsters)
            {
                // MonsterAttack의 public player 변수에 직접 할당
                monster.player = playerTarget;
            }
        }
        else
        {
            Debug.LogError("플레이어 오브젝트 생성 또는 참조 실패! PlayerPrefab이 RoundManager에 할당되었는지 확인하세요.");
        }

        /* 기존 코드: 플레이어 리셋 로직은 삭제하거나 주석 처리
        Playercontroller1 player = FindObjectOfType<Playercontroller1>();
        if (player != null)
        {
            player.ResetPlayer();
        }
        */
    }

    public void LoadNextRound()
    {
        currentRound++;
        if (currentRound < roundDatas.Length)
            LoadRound(roundDatas[currentRound]);
    }

    private void SpawnObjects(GameObject[] prefabs, Vector2[] positions)
    {
        for (int i = 0; i < prefabs.Length && i < positions.Length; i++)
        {
            Instantiate(prefabs[i], positions[i], Quaternion.identity, roundContainer.transform);
        }
    }

    /*public void LoadNextRound()
    {
        currentRound++;
        LoadRound(currentRound);
    }*/
}
