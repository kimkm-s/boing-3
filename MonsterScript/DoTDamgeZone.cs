using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoTDamageZone : MonoBehaviour
{
    [Header("대미지 설정")]
    public int damageAmount = 50;
    public float damageInterval = 1.0f;

    [Header("주기 설정")]
    public float activeDuration = 5.0f;
    public float inactiveDuration = 5.0f;
    public float fadeDuration = 1.0f; // ⭐ 새로 추가: 투명해지는 데 걸리는 시간 (1초)

    private List<HealthWithUI2D> targetsInZone = new List<HealthWithUI2D>();
    private SpriteRenderer spriteRenderer;
    private Coroutine currentFadeRoutine; // 중복 페이드 방지용

    // --- 초기화 및 주기 관리 ---

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GetComponent<Collider2D>() != null && !GetComponent<Collider2D>().isTrigger)
        {
            Debug.LogWarning("DoTDamageZone: Collider2D의 Is Trigger를 활성화해야 합니다.");
        }

        // 초기 시작 시 SpriteRenderer가 있다면 활성화합니다.
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.6f;
            spriteRenderer.color = color;
        }
    }

    void Start()
    {
        StartCoroutine(CycleActivation());
    }

    private IEnumerator CycleActivation()
    {
        while (true)
        {
            // 1. 활성화 (Active) 상태

            // ⭐ 페이드 인 시작 및 충돌체 활성화
            yield return StartCoroutine(SetZoneActive(true));

            // 활성화 시간(5초) 동안 대미지 주기적으로 적용
            float timer = 0f;
            while (timer < activeDuration)
            {
                ApplyDamageToTargets();
                yield return new WaitForSeconds(damageInterval);
                timer += damageInterval;
            }

            // 2. 비활성화 (Inactive) 상태

            // ⭐ 페이드 아웃 시작 및 충돌체 비활성화 (페이드 아웃이 끝날 때까지 기다립니다)
            yield return StartCoroutine(SetZoneActive(false));

            // 비활성화 시간(5초) 동안 대기 (완전히 투명한 상태)
            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    // ⭐ 상태를 설정하고 페이딩을 처리하는 코루틴
    private IEnumerator SetZoneActive(bool isActive)
    {
        // 1. 충돌체 상태는 즉시 변경 (대미지 적용/중단은 즉시 이루어짐)
        GetComponent<Collider2D>().enabled = isActive;
        Debug.Log($"대미지 영역 충돌 상태 변경: {(isActive ? "활성화" : "비활성화")}");

        // 2. 시각적 페이딩 처리
        if (spriteRenderer != null)
        {
            // 목표 알파 값 (활성화: 1.0, 비활성화: 0.0)
            float targetAlpha = isActive ? 0.6f : 0.0f;

            // 기존 페이드 코루틴이 있다면 중지
            if (currentFadeRoutine != null)
            {
                StopCoroutine(currentFadeRoutine);
            }

            // 새로운 페이드 코루틴 시작 후 완료될 때까지 대기
            currentFadeRoutine = StartCoroutine(FadeAlpha(targetAlpha, fadeDuration));
            yield return currentFadeRoutine;
        }
    }

    // ⭐ 서서히 투명도를 변경하는 코루틴
    private IEnumerator FadeAlpha(float targetAlpha, float duration)
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            yield return null; // 다음 프레임까지 대기
        }

        // 정확한 목표 값으로 설정하여 오차 보정
        color.a = targetAlpha;
        spriteRenderer.color = color;

        // 투명해졌을 때 SpriteRenderer 자체를 비활성화 (성능 최적화 및 완벽한 투명)
        if (targetAlpha < 0.1f)
        {
            spriteRenderer.enabled = false;
        }
        else // 불투명해졌을 때 SpriteRenderer 활성화
        {
            spriteRenderer.enabled = true;
        }
    }

    // --- 대미지 적용 로직 및 충돌 감지 로직 (기존 코드와 동일) ---

    private void ApplyDamageToTargets()
    {
        for (int i = targetsInZone.Count - 1; i >= 0; i--)
        {
            HealthWithUI2D target = targetsInZone[i];
            if (target != null)
            {
                target.TakeDamage(damageAmount);
            }
            else
            {
                targetsInZone.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthWithUI2D health = other.GetComponent<HealthWithUI2D>();
            if (health != null && !targetsInZone.Contains(health))
            {
                targetsInZone.Add(health);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthWithUI2D health = other.GetComponent<HealthWithUI2D>();
            if (health != null && targetsInZone.Contains(health))
            {
                targetsInZone.Remove(health);
            }
        }
    }
}