using UnityEngine;
using System.Collections;

public class SnowGimmick : MonoBehaviour
{
    [Header("설산 기믹 설정")]
    public float temperatureDecreaseRate = 2.0f;
    public float decreaseInterval = 0.5f;

    // private TemperatureManager tempManager; // ⭐ 이제 필요 없음
    private Playercontroller1 playerController; // 플레이어 컴포넌트는 여전히 필요
    private Coroutine initializationRoutine;
    private Coroutine temperatureRoutine;

    // =========================================================
    // 1. 활성화 / 비활성화
    // =========================================================

    private void OnEnable()
    {
        Debug.Log("Snow Gimmick: 작동 시작 준비.");
        // ⭐ 초기화 코루틴 시작
        initializationRoutine = StartCoroutine(InitializeGimmickDelayed());
    }

    private void OnDisable()
    {
        Debug.Log("Snow Gimmick: 작동 종료.");
        if (initializationRoutine != null) StopCoroutine(initializationRoutine);
        if (temperatureRoutine != null) StopCoroutine(temperatureRoutine);
    }

    // =========================================================
    // 2. 초기화 (관리자 및 플레이어 연결)
    // =========================================================

    private IEnumerator InitializeGimmickDelayed()
    {
        // ⭐ 1. TemperatureManager 인스턴스가 준비될 때까지 기다립니다.
        while (TemperatureManager.Instance == null)
        {
            yield return null; // 한 프레임 대기
        }

        // ⭐ 2. PlayerController1 컴포넌트 찾기 (기존 로직 유지)
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.2f);
        }

        playerController = player.GetComponent<Playercontroller1>();

        if (playerController != null)
        {
            Debug.Log("Snow Gimmick: 모든 관리자/플레이어 연결 성공. 기믹 로직 시작.");
            temperatureRoutine = StartCoroutine(DecreaseTemperatureOverTime());
        }
        else
        {
            Debug.LogError("SnowGimmick: [초기화 실패] Playercontroller1 컴포넌트가 플레이어에 없습니다.");
        }
    }


    // =========================================================
    // 3. 온도 하강 코루틴 (관리자 접근으로 변경)
    // =========================================================

    private IEnumerator DecreaseTemperatureOverTime()
    {
        TemperatureManager tempManager = TemperatureManager.Instance;

        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);

            // ⭐⭐ 조건 변경: 조준하지 않고 AND 움직이지도 않을 때만 온도 감소 ⭐⭐
            if (playerController.IsAiming == false && playerController.IsMoving == false)
            {
                float amountToDecrease = temperatureDecreaseRate * decreaseInterval;
                tempManager.DecreaseTemperature(amountToDecrease);

                // Debug.Log($"[Snow Gimmick] 온도 하강 적용: -{amountToDecrease:F2}. 현재 온도: {tempManager.Temperature:F1} (정지 상태)");
            }
            // else: 조준 중이거나 움직이는 중일 때는 온도 변화 없음
        }
    }
}