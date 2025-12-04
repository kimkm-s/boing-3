/*using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 8f;

    private Collider2D monsterCollider; // 몬스터 콜라이더 저장

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // 🔥 MonsterAttack에서 호출하는 버전
    public void Init(Vector2 dir, Collider2D ignoreCollider)
    {
        monsterCollider = ignoreCollider;

        // 🔥🔥 중요: monsterCollider가 null이 아닌 경우에만 IgnoreCollision을 호출해야 합니다.
        if (monsterCollider != null)
        {
            // 몬스터 콜라이더와의 충돌만 무시
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), monsterCollider);
        }

        // 방향 이동
        rb.linearVelocity = dir.normalized * speed;

        // 방향 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Wall"인지 확인
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log($"투사체가 {collision.gameObject.name} (Wall)에 충돌하여 파괴됩니다.");

            // 투사체 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D projectileCollider;
    public float speed = 8f;
    public int damageAmount = 20;

    private Collider2D[] monsterColliders;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // MonsterAttack에서 호출
    public void Init(Vector2 dir, Collider2D[] ignoreColliders)
    {
        monsterColliders = ignoreColliders;

        foreach (var col in ignoreColliders)
        {
            if (col != null)
                Physics2D.IgnoreCollision(projectileCollider, col);
        }

        rb.linearVelocity = dir.normalized * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Destroy(gameObject, 5f);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 벽(Wall)과 충돌 시
        if (other.CompareTag("Wall"))
        {
            //Debug.Log($"투사체가 {other.gameObject.name} (Wall)에 닿아 파괴됩니다. (Trigger)");
            Destroy(gameObject);
            return; // Wall과 닿았으니 이후 코드는 실행할 필요 없음
        }

        // 2. 플레이어(Player)와 충돌 시
        // 'Player' 태그를 사용한다고 가정합니다.
        if (other.CompareTag("Player"))
        {
            // 1. 플레이어 오브젝트에서 HealthWithUI2D 컴포넌트 찾기
            HealthWithUI2D playerHealth = other.GetComponent<HealthWithUI2D>();

            // 2. 컴포넌트가 있다면 TakeDamage 호출
            if (playerHealth != null)
            {
                // 투사체의 대미지 값을 설정해야 합니다. (예: 10)
                //int damageAmount = 10; // ⭐ 이 값을 투사체 스크립트의 public 변수로 관리하는 것이 좋습니다.
                playerHealth.TakeDamage(damageAmount);

                Debug.Log($"플레이어에게 {damageAmount} 피해를 입혔습니다. 남은 체력: {playerHealth.CurrentHealth}");
            }

            // 충돌 후 투사체 파괴
            Destroy(gameObject);
            return;
        }
    }
}
