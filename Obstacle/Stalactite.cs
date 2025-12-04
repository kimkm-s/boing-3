using UnityEngine;
using System.Collections; // ⭐ 코루틴을 사용하기 위해 필요합니다.

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Stalactite : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("발사 설정")]
    public float speed = 10f;
    public float lifetime = 5f;
    public float initialDelay = 2.0f; // ⭐ 새로 추가: 대기 시간 2초

    [Header("대미지 설정")]
    public int damageAmount = 30;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 초기 물리 설정: 중력 제거 및 회전 고정 (낙하 시작 전까지 움직임 방지)
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = Vector2.zero; // ⭐ 시작 시 완전히 정지

        // 총 수명: 대기 시간(2초) + 실제 낙하 시간(5초)
        Destroy(gameObject, initialDelay + lifetime);
    }

    void Start()
    {
        // ⭐ Stalactite가 생성되자마자 지연 발사 코루틴을 시작합니다.
        Vector2 initialDirection = Vector2.down; // ⭐ 아래 방향 (0, -1)

        // ⭐ 1. 대기가 시작되기 전에 즉시 최종 방향을 향하도록 회전합니다.
        float angle = Mathf.Atan2(initialDirection.y, initialDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // 2. 코루틴 시작 (딜레이 로직)
        StartCoroutine(DelayedLaunchRoutine(initialDirection));
    }

    // ⭐ 새로 추가: 지연 후 낙하를 실행하는 코루틴
    private IEnumerator DelayedLaunchRoutine(Vector2 direction)
    {
        // 1. 지정된 시간(2초)만큼 대기
        Debug.Log($"투사체 생성. {initialDelay}초 후에 낙하합니다.");
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("정상작동중 - 낙하 시작!");

        // 2. 대기 후 속도 적용 (아래 방향으로 낙하)
        rb.linearVelocity = direction.normalized * speed;

        // 3. 시각적 회전 적용
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // ⭐ OnCollisionEnter2D 함수는 기존 그대로 유지
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ... (충돌 로직은 그대로 유지됩니다)
        GameObject other = collision.gameObject;

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            HealthWithUI2D playerHealth = other.GetComponent<HealthWithUI2D>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
            return;
        }

        HealthWithUI2D wallHealth = other.GetComponent<HealthWithUI2D>();
        if (wallHealth != null)
        {
            wallHealth.TakeDamage(damageAmount);
            Destroy(gameObject);
            return;
        }
    }
}