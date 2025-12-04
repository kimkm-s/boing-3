using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))] // Rigidbody2D 필수!
public class PlayerController : MonoBehaviour
{
    // --- 드래그 입력 변수
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;

    // --- 물리 및 이동 변수
    private Rigidbody2D rb;
    private bool isMoving = false; // 이동 시작 여부

    [Header("발사 관련 설정")]
    public float launchPower = 2f;
    public float dragSensitivity = 1f;
    public float maxSpeed = 10f; // 터널링 방지 보조용 최대 속도 제한

    [Header("반사 관련 설정")]
    public int maxBounceCount = 3;
    private int currentBounceCount;

    [Header("목표물")]
    public Transform goal;
    public float goalRadius = 0.5f;

    // --- 초기 위치 저장
    private readonly Vector2 startPosition = new Vector2(-0.01f, -4.6f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D 설정 (터널링 방지 핵심)
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        // Continuous 모드는 고속 이동 시 터널링을 줄여줍니다.
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        ResetPlayer(); // 초기화 함수를 Start에서 호출
    }

    void Update()
    {
        HandleInput();

        // R키 입력 체크 → 위치 초기화
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }
    }

    void FixedUpdate()
    {
        // isMoving 상태를 rb.velocity로 업데이트
        isMoving = rb.linearVelocity.sqrMagnitude > 0.001f;

        if (!isMoving) return;

        // 속도 제한 (터널링 방지 보조)
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        CheckGoal(); // 목표물 체크
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // isMoving 상태가 아니면 충돌 처리 무시 (발사 전)
        if (!isMoving) return;

        // 충돌 지점의 법선 벡터를 가져옵니다.
        // 이것이 충돌이 일어난 벽면의 방향(반사각 계산의 기준)입니다.
        Vector2 normal = collision.contacts[0].normal;

        // Wall 태그 처리
        if (collision.collider.CompareTag("Wall"))
        {
            if (currentBounceCount > 0)
            {
                // ⭐ 코드로 반사 강제 구현 ⭐
                // 현재 속도를 법선 기준으로 반사
                rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);

                currentBounceCount--;
                Debug.Log("벽 반사 (Code Reflect). 남은 횟수: " + currentBounceCount);
            }
            else
            {
                // 튕김 횟수 소진 → 멈춤 및 실패 처리
                StopMovement();
                GameManager.Instance.OnLose();
            }
        }
        // InvisibleWall 태그 처리
        else if (collision.collider.CompareTag("InvisibleWall"))
        {
            // InvisibleWall은 '반사 횟수가 0일 때' 멈춤 및 실패
            if (currentBounceCount == 0)
            {
                StopMovement();
                GameManager.Instance.OnLose();
            }
            else
            {
                // 횟수가 남았으면 충돌 감지 후에도 그냥 통과 (반사 없음)
                // 충돌 처리 후 속도를 유지하여 이동을 지속
            }
        }
        // Goal 태그 처리
        else if (collision.collider.CompareTag("Goal"))
        {
            // Goal 충돌 시 처리
            if (currentBounceCount == 0)
            {
                StopMovement();
                GameManager.Instance.OnWin();
            }
            else
            {
                Debug.Log("목표 충돌했으나 반사 횟수 남음 (무시/통과)");
                // 횟수가 남아있으면 충돌 감지 후에도 그냥 통과 (반사 없음)
            }
        }
    }

    // 플레이어 초기화 함수
    private void ResetPlayer()
    {
        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        isDragging = false;
        currentBounceCount = maxBounceCount;
    }

    // 이동 정지 처리
    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }

    private void HandleInput()
    {
        if (isMoving) return; // 움직이는 중에는 입력 무시

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
            currentBounceCount = maxBounceCount; // 드래그 시작 시 횟수 초기화
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDir = (dragStartPos - dragEndPos).normalized;
            float rawPower = (dragStartPos - dragEndPos).magnitude;

            // 발사 힘 계산 (최대 속도를 초과하지 않도록 제한)
            float finalPower = Mathf.Clamp(rawPower * launchPower * dragSensitivity, 0f, maxSpeed);

            // Rigidbody에 속도 적용
            rb.linearVelocity = launchDir * finalPower;
            isDragging = false;
        }
    }

    private void CheckGoal()
    {
        if (goal == null) return;

        // 목표물 반경 체크 (Goal Collider 사용 안하고 범위 체크)
        float dist = Vector2.Distance(rb.position, goal.position);
        if (dist < goalRadius)
        {
            if (currentBounceCount == 0)
            {
                StopMovement();
                GameManager.Instance.OnWin();
            }
            // 횟수가 남았으면 아직 승리 조건이 아니므로 계속 진행
        }
    }
}