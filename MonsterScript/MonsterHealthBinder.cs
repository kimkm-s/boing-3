using UnityEngine;

public class MonsterHealthBinder : MonoBehaviour
{

    private void Start()
    {
        var health = GetComponent<HealthWithUI2D>();

        health.OnDiedCallback = () =>
        {
            Debug.Log("ÆÄ±«´Â µÆ´Âµ¥¿ä");
            Destroy(gameObject);
        };
    }
}
