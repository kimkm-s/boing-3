using UnityEngine;

public class MapGimmickManager : MonoBehaviour
{
    [Header("현재 스테이지 설정")]
    // Inspector에서 현재 스테이지의 기믹을 선택합니다.
    public StageGimmickType currentGimmickType = StageGimmickType.None;

    // 모든 기믹 컴포넌트를 저장하는 변수
    // 현재는 간단하게 Inspector에서 수동 연결하는 방식으로 구현합니다.
    [Header("기믹 컴포넌트 연결")]
    public DesertGimmick desertGimmick;
    public SnowGimmick snowGimmick;
    public CaveGimmick caveGimmick;

    private void Start()
    {
        InitializeGimmicks();
    }

    private void InitializeGimmicks()
    {
        // 1. 모든 기믹 컴포넌트를 비활성화 상태로 초기화합니다.
        // 이는 중복 작동을 막기 위함입니다.
        if (desertGimmick != null) desertGimmick.gameObject.SetActive(false);
        if (snowGimmick != null) snowGimmick.gameObject.SetActive(false);
        if (caveGimmick != null) caveGimmick.gameObject.SetActive(false);

        // 2. 현재 설정된 기믹 타입에 따라 해당 기믹을 활성화합니다.
        // 기믹 스크립트 자체는 활성화될 때 Start/OnEnable에서 자신의 로직을 시작합니다.
        switch (currentGimmickType)
        {
            case StageGimmickType.Desert_Sinking:
                if (desertGimmick != null)
                {
                    desertGimmick.gameObject.SetActive(true);
                    Debug.Log("사막 기믹 활성화: 서서히 가라앉는 효과 적용.");
                    // 필요하다면 여기서 desertGimmick.Init(초기값) 등을 호출할 수 있습니다.
                }
                break;

            case StageGimmickType.Snow_Freezing:
                if (snowGimmick != null)
                {
                    snowGimmick.gameObject.SetActive(true);
                    Debug.Log("설산 기믹 활성화: 빙결 효과 적용.");
                }
                break;

            case StageGimmickType.Cave_Darkness:
                if (caveGimmick != null)
                {
                    caveGimmick.gameObject.SetActive(true);
                    Debug.Log("동굴 기믹 활성화: 어두운 효과 적용.");
                }
                break;

            case StageGimmickType.None:
                Debug.Log("현재 스테이지에는 특수 기믹이 없습니다.");
                break;
        }
    }

    // 다른 스크립트에서 현재 기믹 상태를 문의할 수 있는 Public Getter
    public StageGimmickType GetCurrentGimmickType()
    {
        return currentGimmickType;
    }
}