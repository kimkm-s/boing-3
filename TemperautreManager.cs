using UnityEngine;

public enum DamageAttribute
{
    Fire,
    Ice,
    Normal
}

public class TemperatureManager : MonoBehaviour
{
    // ⭐⭐ 싱글톤 인스턴스 추가 ⭐⭐
    public static TemperatureManager Instance { get; private set; }

    public float Temperature { get; private set; } = 20f;
    public float MinTemp = -50f;
    public float MaxTemp = 100f;

    // 상태
    public bool HotBuff { get; private set; }
    public bool ColdDebuff { get; private set; }
    public bool HeatOverload { get; private set; }
    public bool Frostbite { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 씬 전환에도 유지하고 싶다면: DontDestroyOnLoad(gameObject);
            Debug.Log("TemperatureManager가 관리자 오브젝트에 성공적으로 초기화되었습니다.");
        }
    }

    public void IncreaseTemperature(float amount)
    {
        Temperature = Mathf.Clamp(Temperature + amount, MinTemp, MaxTemp);
        UpdateStates();
    }

    public void DecreaseTemperature(float amount)
    {
        Temperature = Mathf.Clamp(Temperature - amount, MinTemp, MaxTemp);
        UpdateStates();
    }

    public void ApplyHit(DamageAttribute attr)
    {
        switch (attr)
        {
            case DamageAttribute.Fire:
                IncreaseTemperature(20f);
                // Fire DoT 부여 로직 (추후 확장)
                break;

            case DamageAttribute.Ice:
                DecreaseTemperature(20f);
                // Ice Slow 로직 (추후 확장)
                break;

            case DamageAttribute.Normal:
                // 기본 온도 변화 없음
                break;
        }
    }

    private void UpdateStates()
    {
        HotBuff = Temperature >= 70f;
        ColdDebuff = Temperature <= 0f;

        HeatOverload = Temperature >= 90f;
        Frostbite = Temperature <= -20f;
    }

    public float CalculateBounceMultiplier()
    {
        if (Temperature >= 70f) return 1.5f;
        if (Temperature <= 0f) return 0.7f;
        return 1.0f;
    }
}
