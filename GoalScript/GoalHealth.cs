using UnityEngine;

public class GoalHealth : MonoBehaviour
{
    public bool IsDead { get; private set; } = false;

    // HealthWithUI2D에서 Death 이벤트 호출 시 실행
    public void OnDeath()
    {
        IsDead = true;
        Debug.Log("Goal is Dead!");
    }
}
