using UnityEngine;

public class CaveGimmick : MonoBehaviour
{
    [Header("동굴 기믹 설정")]
    // 이 값은 전역 조명(Global Light)의 강도를 설정하는 데 사용될 수 있습니다.
    public float targetGlobalLightIntensity = 0.1f;
    public Color ambientColor = new Color(0.1f, 0.1f, 0.1f); // 어두운 주변 색상

    // MapGimmickManager에 의해 오브젝트가 활성화되면 호출됩니다.
    private void OnEnable()
    {
        Debug.Log("Cave Gimmick: 작동 시작. 어둠 효과를 적용합니다.");

        // ⭐ 여기에 씬의 전역 조명(Global Lighting) 설정을 변경하는 코드를 작성합니다.
        // 예: GlobalLightingManager.SetIntensity(targetGlobalLightIntensity);

        // 플레이어에게 시야 확보용 작은 광원(Light Component)을 추가하는 것도 고려할 수 있습니다.
    }

    // MapGimmickManager에 의해 오브젝트가 비활성화되면 호출됩니다.
    private void OnDisable()
    {
        Debug.Log("Cave Gimmick: 작동 종료. 조명 설정을 원래대로 복구합니다.");

        // ⭐ 씬의 조명 설정을 원래 값으로 되돌리는 정리 작업을 수행합니다.
        // 예: GlobalLightingManager.ResetIntensity();
    }
}