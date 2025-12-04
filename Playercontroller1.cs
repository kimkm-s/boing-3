using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Playercontroller1 : MonoBehaviour
{
    // --- 드래그 입력 변수
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;

    // --- 물리 및 이동 변수
    private Rigidbody2D rb;
    private bool isMoving = false;

    [Header("발사 관련 설정")]
    public float launchPower = 2f;
    public float dragSensitivity = 1f;
    public float maxSpeed = 10f;

    // ⭐⭐ 온도 상태 연동 변수/속성 ⭐⭐
    public bool IsAiming { get; private set; } = false; // 조준 상태
    private float originalLaunchPower; // 원본 발사력을 저장할 변수
    public float OriginalLaunchPower => originalLaunchPower; // 외부 접근용 속성

    [Header("반사 관련 설정")]
    public int maxBounceCount = 3;
    private int currentBounceCount;

    [Header("목표물")]
    public Transform goal;
    public float goalRadius = 0.5f;

    private float lastBounceTime = 0f;
    private float bounceCooldown = 0.05f;

    private readonly Vector2 startPosition = new Vector2(-0.01f, -4.6f);

    private LineRenderer lineRenderer;
    public float aimLineMaxLength = 3f;
    public float aimLineWidth = 0.05f;

    public bool IsMoving => isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = aimLineWidth;
        lineRenderer.endWidth = aimLineWidth;
        lineRenderer.enabled = false;
        lineRenderer.sortingOrder = 10;

        // ⭐ Awake에서 원본 값 저장
        originalLaunchPower = launchPower;
    }

    void Start()
    {
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        ResetPlayer();
    }

    void Update()
    {
        // 빙결 등의 외부 효과로 인해 launchPower가 0이면 입력 처리 무시 (HandleInput 내부에서 처리되므로 주석 처리)
        // if (launchPower <= 0f && !isMoving) return; 

        HandleInput();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }
    }

    void FixedUpdate()
    {
        isMoving = rb.linearVelocity.sqrMagnitude > 0.001f;

        if (!isMoving) return;

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        CheckGoal();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMoving) return;

        // ... (기존 충돌 처리 로직 유지) ...
        Vector2 normal = collision.contacts[0].normal;

        if (collision.collider.CompareTag("Wall"))
        {
            if (currentBounceCount > 0)
            {
                if (Time.time - lastBounceTime > bounceCooldown)
                {
                    rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);

                    if (rb.linearVelocity.magnitude < 1.0f)
                        rb.linearVelocity = rb.linearVelocity.normalized * 1.0f;

                    currentBounceCount--;
                    lastBounceTime = Time.time;
                }
            }
            else
            {
                StopMovement();
            }
        }
        else if (collision.collider.CompareTag("InvisibleWall"))
        {
            if (currentBounceCount == 0)
            {
                StopMovement();
                //GameManager.Instance.OnLose();
            }
            // else: 횟수가 남았으면 충돌 감지 후에도 그냥 통과 (반사 없음)
        }
        else if (collision.collider.CompareTag("Goal"))
        {
            // ... (기존 Goal 충돌 처리 로직 유지) ...
            // HealthWithUI2D goalHealth = collision.collider.GetComponent<HealthWithUI2D>();
            // if (goalHealth == null) { Debug.LogError("Goal에 HealthWithUI2D 스크립트가 없음!"); return; }

            // if (goalHealth.CurrentHealth <= 0) { StopMovement(); GameManager.Instance.OnWin(); return; }

            // goalHealth.TakeDamage(1);

            // if (goalHealth.CurrentHealth <= 0) { StopMovement(); GameManager.Instance.OnWin(); }
            // else { Debug.Log("Goal 피격했지만 아직 살아있음."); }
        }
    }


    public void ResetPlayer()
    {
        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        isDragging = false;
        currentBounceCount = maxBounceCount;
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }

    // ⭐⭐ 외부 상태 관리 스크립트에서 호출할 수 있도록 public 선언 ⭐⭐
    public void StopAiming()
    {
        if (IsAiming)
        {
            IsAiming = false;
            isDragging = false;
            lineRenderer.enabled = false;
            Debug.Log("조준이 외부 요인(빙결)에 의해 강제 해제되었습니다.");
        }
    }

    private void HandleInput()
    {
        // 발사력이 0이면 조준/발사 입력 자체를 막음 (빙결 효과)
        if (launchPower <= 0f) return;

        if (isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
            // ⭐ 조준 시작 시
            IsAiming = true;

            currentBounceCount = maxBounceCount;
            lineRenderer.enabled = true;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchVector = dragStartPos - currentPos;
            Vector2 aimDirection = launchVector.normalized;
            float lineLength = Mathf.Min(launchVector.magnitude, aimLineMaxLength);

            Vector3 startPos3D = transform.position;
            startPos3D.z = -1f;

            Vector3 lineEnd3D = (Vector3)((Vector2)transform.position + aimDirection * lineLength);
            lineEnd3D.z = -1f;

            lineRenderer.SetPosition(0, startPos3D);
            lineRenderer.SetPosition(1, lineEnd3D);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDir = (dragStartPos - dragEndPos).normalized;
            float rawPower = (dragStartPos - dragEndPos).magnitude;

            float finalPower = Mathf.Clamp(rawPower * launchPower * dragSensitivity, 0f, maxSpeed);

            rb.linearVelocity = launchDir * finalPower;
            isDragging = false;
            // ⭐ 발사 완료 시
            IsAiming = false;

            lineRenderer.enabled = false;
        }
    }


    private void CheckGoal()
    {
        // ... (기존 목표물 체크 로직 유지) ...
    }
}