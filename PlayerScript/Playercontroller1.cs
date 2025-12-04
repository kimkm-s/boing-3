using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))] // Rigidbody2D 필수!
public class Playercontroller1 : MonoBehaviour
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

    private float lastBounceTime = 0f;
    private float bounceCooldown = 0.05f; // 50ms 쿨타임

    // --- 초기 위치 저장
    private readonly Vector2 startPosition = new Vector2(-0.01f, -4.6f);

    private LineRenderer lineRenderer;
    public float aimLineMaxLength = 3f;   // 조준선 최대 길이
    public float aimLineWidth = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = aimLineWidth;
        lineRenderer.endWidth = aimLineWidth;
        lineRenderer.enabled = false;  // 처음에는 숨김
        lineRenderer.sortingOrder = 10; // UI보다 위로 나오게(원하는 값)
    }

    void Start()
    {

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

        // 충돌 지점의 법선 벡터 가져오기
        Vector2 normal = collision.contacts[0].normal;

        // Wall 태그 처리
        if (collision.collider.CompareTag("Wall"))
        {
            if (currentBounceCount > 0)
            {
                if (Time.time - lastBounceTime > bounceCooldown)
                {
                    // ⭐ 코드로 반사 강제 구현 ⭐
                    rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);

                    // ⭐ 최소 속도 보정
                    if (rb.linearVelocity.magnitude < 1.0f)
                        rb.linearVelocity = rb.linearVelocity.normalized * 1.0f;

                    currentBounceCount--;
                    lastBounceTime = Time.time; // 마지막 반사 시간 갱신
                }
            }
            else
            {
                StopMovement();
            }
        }
        // InvisibleWall 태그 처리
        else if (collision.collider.CompareTag("InvisibleWall"))
        {
            // InvisibleWall은 '반사 횟수가 0일 때' 멈춤 및 실패
            if (currentBounceCount == 0) 
            {
                StopMovement();
                //GameManager.Instance.OnLose();
            }
            else
            {
                // 횟수가 남았으면 충돌 감지 후에도 그냥 통과 (반사 없음)
            }
        }
        // Goal 태그 처리
        else if (collision.collider.CompareTag("Goal"))
        {
            // HealthWithUI2D 컴포넌트를 가져오기
            HealthWithUI2D goalHealth = collision.collider.GetComponent<HealthWithUI2D>();
            if (goalHealth == null)
            {
                Debug.LogError("Goal에 HealthWithUI2D 스크립트가 없음!");
                return;
            }

            // 이미 죽었는지 체크
            if (goalHealth.CurrentHealth <= 0)
            {
                StopMovement();
                GameManager.Instance.OnWin();
                return;
            }

            // 대미지 주기
            goalHealth.TakeDamage(1);

            // 대미지 이후 죽었는지 체크
            if (goalHealth.CurrentHealth <= 0)
            {
                StopMovement();
                GameManager.Instance.OnWin();
            }
            else
            {
                Debug.Log("Goal 피격했지만 아직 살아있음.");
            }
        }
    }


    // 플레이어 초기화 함수
    public void ResetPlayer()
    {
        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;  // linearVelocity -> velocity
        isMoving = false;
        isDragging = false;
        currentBounceCount = maxBounceCount;
    }

    // 이동 정지 처리
    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;  // linearVelocity -> velocity
        isMoving = false;
    }

    private void HandleInput()
    {

        if (isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
            currentBounceCount = maxBounceCount;

            lineRenderer.enabled = true;
        }

        // ⭐ 드래그 중 — 조준선 업데이트 (발사 방향과 일치하도록 수정)
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 1. 🚀 발사 방향 벡터 계산: (드래그 시작 지점 - 현재 마우스 위치)
            //    * 이 방향이 플레이어가 실제로 발사될 방향입니다.
            Vector2 launchVector = dragStartPos - currentPos;

            // 2. 방향 및 길이 계산
            Vector2 aimDirection = launchVector.normalized;

            // 조준선 길이를 드래그 강도에 따라 제한
            float lineLength = Mathf.Min(launchVector.magnitude, aimLineMaxLength);

            // 3. 조준선 시작/끝점 계산

            // 시작점 (Z축 강제 조정 - 이전 답변에서 다룬 문제 해결)
            Vector3 startPos3D = transform.position;
            startPos3D.z = -1f; // 카메라 앞으로 당기기

            // 끝점: 플레이어 위치 + (발사 방향 * 길이)
            Vector3 lineEnd3D = (Vector3)((Vector2)transform.position + aimDirection * lineLength);
            lineEnd3D.z = -1f; // 카메라 앞으로 당기기

            // 4. LineRenderer에 적용
            lineRenderer.SetPosition(0, startPos3D);  // 시작점 = 플레이어
            lineRenderer.SetPosition(1, lineEnd3D);   // 끝점 = 발사 방향으로 연장
        }


        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDir = (dragStartPos - dragEndPos).normalized;
            float rawPower = (dragStartPos - dragEndPos).magnitude;

            float finalPower = Mathf.Clamp(rawPower * launchPower * dragSensitivity, 0f, maxSpeed);

            rb.linearVelocity = launchDir * finalPower;  // linearVelocity -> velocity
            isDragging = false;

            lineRenderer.enabled = false;
        }


    }


    private void CheckGoal()
    {
        if (goal == null) return;

        float dist = Vector2.Distance(rb.position, goal.position);
        if (dist < goalRadius && currentBounceCount == 0)
        {
            StopMovement();
            GameManager.Instance.OnWin();
        }
    }
}