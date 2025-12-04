using System.Collections;
using UnityEngine;

public class DesertGimmick : MonoBehaviour
{
    // ⭐ 플레이어 상태 정의
    private enum PlayerState
    {
        IDLE,       // 멈추거나 움직이고 있는 일반 상태
        ENTERING,   // 가라앉는 애니메이션/연출 중
        SUNK,       // 가라앉는 애니메이션 완료, 기믹 효과 적용 중
        EXITING     // 모래에서 나오는 애니메이션/연출 중
    }

    [Header("사막 기믹 설정")]
    public float animationDuration = 1.0f;   // ⭐ 애니메이션(디버그) 연출 시간
    public float positionTolerance = 0.01f;  // 정지 상태로 간주할 최소 위치 변화 허용 오차

    [Header("바닥 접촉 설정")]
    public LayerMask desertGroundLayer;      // 사막 바닥을 식별할 레이어 마스크

    // --- Private 변수 ---
    private Rigidbody2D playerRb;
    private Collider2D playerCollider;
    private Coroutine initializationRoutine;
    private Vector2 previousPosition;
    private bool isGimmickReady = false;

    [Header("바닥 감지 설정")]
    public Transform groundCheckPoint; // ⭐ 플레이어 발밑에 배치할 Empty 오브젝트의 Transform
    public float checkRadius = 0.1f;  // ⭐ 감지 원의 반지름 (콜라이더보다 조금 크게 설정)

    // ⭐ isSinking 대신 상태 변수 사용
    private PlayerState playerCurrentState = PlayerState.IDLE;
    private Coroutine transitionRoutine;     // 애니메이션 재생 코루틴

    // =========================================================
    // 1. 활성화 / 비활성화 (로직 유지)
    // =========================================================

    private void OnEnable()
    {
        Debug.Log("Desert Gimmick: 작동 시작 준비.");
        initializationRoutine = StartCoroutine(InitializeGimmickDelayed());
    }

    private void OnDisable()
    {
        Debug.Log("Desert Gimmick: 작동 종료.");

        if (initializationRoutine != null) StopCoroutine(initializationRoutine);
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);

        isGimmickReady = false;
        playerCurrentState = PlayerState.IDLE; // 종료 시 상태 초기화

        playerRb = null;
        playerCollider = null;
    }

    // =========================================================
    // 2. 초기화 (로직 유지)
    // =========================================================

    private IEnumerator InitializeGimmickDelayed()
    {
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.2f);
        }

        playerRb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();

        // ⭐⭐ 수정된 부분: 런타임에 자식 오브젝트 찾기 ⭐⭐
        Transform groundCheckCandidate = player.transform.Find("groundCheckPoint"); // "Foot" 대신 실제 이름을 사용하세요!

        if (playerRb == null || playerCollider == null || groundCheckCandidate == null)
        {
            // GroundCheckPoint를 찾지 못할 경우 에러 출력
            Debug.LogError("DesertGimmick: Rigidbody2D, Collider2D, 또는 'groundCheckPoint' 자식 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        // 찾은 Transform을 groundCheckPoint 변수에 할당
        groundCheckPoint = groundCheckCandidate;
        // ⭐⭐⭐ 이제 Inspector에서 연결할 필요가 없습니다. ⭐⭐⭐

        previousPosition = playerRb.position;
        playerCurrentState = PlayerState.IDLE;
        isGimmickReady = true;
        Debug.Log("Desert Gimmick: 플레이어 연결 성공, 기믹 감지 시작.");
    }

    // =========================================================
    // 3. 바닥 접촉 확인 헬퍼 함수 (로직 유지)
    // =========================================================

    private bool IsOnDesertGround()
    {
        if (groundCheckPoint == null)
        {
            // groundCheckPoint가 설정되지 않았다면 기본 collider로 체크 (혹은 false 반환)
            return playerCollider.IsTouchingLayers(desertGroundLayer);
        }

        // ⭐ Physics2D.OverlapCircle을 사용하여 특정 레이어와의 접촉을 확인합니다.
        Collider2D hit = Physics2D.OverlapCircle(groundCheckPoint.position, checkRadius, desertGroundLayer);

        // hit이 null이 아니면(무언가를 감지했다면) true 반환
        return hit != null;
    }

    // =========================================================
    // 4. 메인 로직 (FixedUpdate: 상태 천이(Transition) 감지)
    // =========================================================

    private void FixedUpdate()
    {
        if (playerRb == null || !isGimmickReady) return;

        Vector2 currentPosition = playerRb.position;
        float distanceMoved = Vector2.Distance(currentPosition, previousPosition);

        bool isStillAndOnGround = distanceMoved < positionTolerance && IsOnDesertGround();

        // 1. 상태 천이 (Transition) 로직
        switch (playerCurrentState)
        {
            case PlayerState.IDLE:
                // IDLE 상태에서 멈춰있고 바닥에 닿으면 진입 시작
                if (isStillAndOnGround)
                {
                    transitionRoutine = StartCoroutine(HandleAnimationTransition(PlayerState.ENTERING));
                }
                break;

            case PlayerState.SUNK:
                // SUNK 상태에서 움직이거나 공중에 뜨면 복귀 시작
                if (!isStillAndOnGround)
                {
                    transitionRoutine = StartCoroutine(HandleAnimationTransition(PlayerState.EXITING));
                }
                break;

            case PlayerState.ENTERING:
            case PlayerState.EXITING:
                // 애니메이션이 재생되는 동안은 FixedUpdate에서 아무것도 하지 않음 (⭐ 반복 출력 해결)
                // 코루틴(HandleAnimationTransition)이 완료될 때까지 상태를 유지
                break;
        }

        // 2. 기믹 효과 적용 시점 (SUNK 상태일 때만)
        if (playerCurrentState == PlayerState.SUNK)
        {
            Debug.Log("APPLYING GIMMICK EFFECT: 모래에 빠져 효과를 받고 있습니다. (SUNK)");
        }

        previousPosition = currentPosition;
    }

    // =========================================================
    // 5. 애니메이션 처리 코루틴 (DEBUG 버전)
    // =========================================================

    private IEnumerator HandleAnimationTransition(PlayerState targetState)
    {
        // ⭐ 1. 상태 변경: 애니메이션 시작과 동시에 상태를 잠금
        playerCurrentState = targetState;

        if (targetState == PlayerState.ENTERING)
        {
            // ⭐ Animator.Play("SinkingClip") 호출
            Debug.Log(">>> [ENTERING START] 가라앉는 애니메이션 재생 시작 (Debug). " + animationDuration + "초 후 완료.");
        }
        else if (targetState == PlayerState.EXITING)
        {
            // ⭐ Animator.Play("BasicSpriteClip") 호출 (기본 스프라이트로 복귀)
            Debug.Log(">>> [EXITING START] 기본 스프라이트로 복귀 애니메이션 재생 (Debug). " + animationDuration + "초 후 완료.");
        }

        // 2. 애니메이션 연출 시간만큼 지연
        yield return new WaitForSeconds(animationDuration);

        // 3. 애니메이션 완료 후 최종 상태 확정
        if (targetState == PlayerState.ENTERING)
        {
            playerCurrentState = PlayerState.SUNK; // 가라앉기 완료 -> 효과 적용 상태
            Debug.Log("Desert Gimmick: 애니메이션 완료. 최종 상태: SUNK!");
        }
        else // EXITING 완료
        {
            playerCurrentState = PlayerState.IDLE; // 복귀 완료 -> 일반 상태
            transitionRoutine = null; // 코루틴 참조 해제
            Debug.Log("Desert Gimmick: 애니메이션 완료. 최종 상태: IDLE!");
        }
    }
}