using UnityEngine;

public class SnowGimmick : MonoBehaviour
{
    [Header("설산 기믹 설정")]
    public float freezeRate = 0.05f; // 빙결 게이지가 차오르는 속도
    public float maxFreezeTime = 5.0f; // 최대 빙결 시간

    // MapGimmickManager에 의해 오브젝트가 활성화되면 호출됩니다.
    private void OnEnable()
    {
        Debug.Log("Snow Gimmick: 작동 시작. 빙결 효과 로직을 준비합니다.");

        // ⭐ 여기에 플레이어 상태 확인 및 빙결 로직 시작 코드를 작성합니다.
        // 예: PlayerController.StartFreezingCheck(freezeRate);
    }

    // MapGimmickManager에 의해 오브젝트가 비활성화되면 호출됩니다.
    private void OnDisable()
    {
        Debug.Log("Snow Gimmick: 작동 종료. 빙결 효과를 해제합니다.");

        // ⭐ 기믹이 종료될 때 필요한 정리 작업을 수행합니다.
        // 예: PlayerController.StopFreezingCheck();
    }

    // 이 Update는 플레이어의 상태(조준 여부)를 지속적으로 확인하는 데 사용될 수 있습니다.
    /*
    void Update()
    {
        // if (플레이어가 조준 중이 아니라면)
        // {
        //     플레이어에게 빙결 효과(예: 슬로우)를 적용합니다.
        // }
    }
    */
}