using UnityEngine;

public class PlayerHealthBinder : MonoBehaviour
{
    private void Start()
    {
        var health = GetComponent<HealthWithUI2D>();

        health.OnDiedCallback = () =>
        {
            GameManager.Instance.OnLose();
            Destroy(gameObject);
        };
    }
}
