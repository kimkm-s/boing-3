using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthWithUI2D))]
[RequireComponent(typeof(Collider2D))]
public class TemporaryWall : MonoBehaviour
{
    // ... (기존 변수 및 Awake, ResetWall 함수는 동일) ...
    [Header("설정")]
    public float respawnTime = 3.0f;

    // ⭐ 벽이 플레이어와 충돌 시 받을 대미지 설정
    [Header("대미지 설정")]
    public int damageReceivedOnHit = 50; // 플레이어와 충돌 시 벽이 받는 대미지 양
    private float lastHitTime = 0f;
    private float hitCooldown = 0.5f; // 연속 대미지 방지 쿨타임 (물리 충돌 시 필수)

    private HealthWithUI2D healthComponent;
    private Collider2D wallCollider;
    private SpriteRenderer spriteRenderer;
    private Coroutine respawnCoroutine;

    void Awake()
    {
        healthComponent = GetComponent<HealthWithUI2D>();
        wallCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("TemporaryWall 스크립트는 SpriteRenderer를 필요로 합니다. 확인해주세요!");
        }
    }

    // ⭐ 단단한 물리 충돌 감지 (Is Trigger = false)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 충돌 쿨타임 체크 (연속 대미지 방지)
        if (Time.time < lastHitTime + hitCooldown)
        {
            return;
        }

        // 2. 충돌한 오브젝트의 태그가 "Player"인지 확인 (플레이어 충돌만 감지)
        if (collision.gameObject.CompareTag("Player"))
        {
            // 3. 벽 자신의 체력 컴포넌트에 대미지를 요청합니다.
            if (healthComponent != null)
            {
                // 벽이 대미지를 받습니다!
                healthComponent.TakeDamage(damageReceivedOnHit);
                Debug.Log(gameObject.name + "이 Player와 충돌하여 대미지를 받았습니다. 남은 체력: " + healthComponent.CurrentHealth);

                lastHitTime = Time.time; // 쿨타임 갱신
            }
        }
    }

    // 외부(HealthWithUI2D의 TakeDamage 함수)에서 체력이 0이 되었을 때 호출하는 함수
    public void OnWallDestroyed()
    {
        // ⭐ Null 체크 추가: 컴포넌트가 유효한지 확인
        if (healthComponent == null || healthComponent.CurrentHealth > 0)
        {
            return;
        }

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        respawnCoroutine = StartCoroutine(RespawnWallAfterTime(respawnTime));
    }

    private IEnumerator RespawnWallAfterTime(float delay)
    {
        // 1. 충돌 판정 끄기
        if (wallCollider != null) wallCollider.enabled = false;

        // 2. 투명화 (렌더러 끄기)
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        Debug.Log(gameObject.name + " 파괴됨. " + delay + "초 후 재생성됩니다.");

        yield return new WaitForSeconds(delay);

        // ⭐ Null 체크 추가: 오브젝트가 그 사이에 파괴되지 않았는지 확인
        if (gameObject == null) yield break;

        // 4. 충돌 판정 다시 켜기
        if (wallCollider != null) wallCollider.enabled = true;

        // 5. 투명화 해제 (렌더러 다시 켜기)
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        // 6. 체력 회복
        if (healthComponent != null) healthComponent.ResetHealth();

        Debug.Log(gameObject.name + " 재생성 완료.");

        respawnCoroutine = null;
    }

    // ... (ResetWall 함수도 필요하다면 Null 체크를 추가하여 사용) ...
}