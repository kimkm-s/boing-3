using UnityEngine;

public class PlayerTemperatureStatus : MonoBehaviour
{
    private Playercontroller1 playerController;
    private SpriteRenderer spriteRenderer;

    [Header("온도 효과 설정")]
    public float frostbiteMovementMultiplier = 0.0f;
    public Color normalColor = Color.white;
    public Color frozenColor = Color.blue;

    [Header("시간 관리 설정")]
    public float effectDuration = 3.0f;         // ⭐ 빙결 효과가 지속될 시간 (3초로 가정)
    public float releaseCooldownTime = 5.0f;    // ⭐ 해제 후 면역 쿨다운 시간

    private float effectReleaseTime = 0.0f;     // 효과가 자동으로 해제될 시간 (duration 관리)
    private float nextApplyTime = 0.0f;         // 다음 빙결이 적용될 수 있는 시간 (쿨다운 관리)
    private bool isEffectActive = false;        // 현재 빙결 효과가 적용 중인지 추적

    private void Start()
    {
        // ... (기존 Start 로직 유지: GetComponent 및 null 체크) ...
        playerController = GetComponent<Playercontroller1>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ... (null 체크 및 normalColor 설정 유지) ...
    }

    private void Update()
    {
        if (TemperatureManager.Instance == null || playerController == null)
        {
            return;
        }

        // =======================================================
        // 1. 효과 적용 로직 (온도 임계점 이하)
        // =======================================================
        if (TemperatureManager.Instance.Frostbite)
        {
            // 빙결이 시작되어야 할 때: 효과가 비활성이고, 재적용 쿨다운이 끝났다면
            if (!isEffectActive && Time.time >= nextApplyTime)
            {
                ApplyFrostbiteEffect();
                isEffectActive = true;

                // ⭐⭐ 효과 지속 시간 설정 ⭐⭐
                effectReleaseTime = Time.time + effectDuration;
            }
            // 쿨다운 중이거나 이미 활성화 상태라면 아무것도 하지 않음 (면역)
        }

        // =======================================================
        // 2. 효과 해제 로직 (시간 기반)
        // =======================================================
        // 현재 효과가 적용 중이고, 정해진 해제 시간이 되었거나 시간이 초과되었다면
        if (isEffectActive && Time.time >= effectReleaseTime)
        {
            RemoveFrostbiteEffect();

            // ⭐⭐ 해제 후 면역 쿨다운 시작 ⭐⭐
            nextApplyTime = Time.time + releaseCooldownTime;

            isEffectActive = false;
            Debug.Log($"빙결 효과 해제(시간 만료): {releaseCooldownTime}초 동안 면역 상태가 적용됩니다.");
        }

        // ... (쿨다운 디버그 로그는 필요 시 활성화) ...
    }

    private void ApplyFrostbiteEffect()
    {
        // ... (기존 ApplyFrostbiteEffect 로직 유지) ...
        if (playerController.IsAiming)
        {
            playerController.StopAiming();
        }
        playerController.launchPower = frostbiteMovementMultiplier;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = frozenColor;
        }
        Debug.Log("Frostbite 적용: 플레이어 조작이 제한됩니다.");
    }

    private void RemoveFrostbiteEffect()
    {
        // ... (기존 RemoveFrostbiteEffect 로직 유지) ...
        playerController.launchPower = playerController.OriginalLaunchPower;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
        Debug.Log("Frostbite 해제: launchPower 복구.");
    }
}