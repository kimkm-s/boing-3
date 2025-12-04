using UnityEngine;

public class GoalHealthBinder : MonoBehaviour
{
    private void Start()
    {
        var health = GetComponent<HealthWithUI2D>();
        var goal = GetComponent<GoalHealth>();

        health.OnDiedCallback = () =>
        {
            goal.OnDeath();
            GameManager.Instance.RoundWin();
            Destroy(gameObject);
        };
    }
}
