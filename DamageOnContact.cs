using UnityEngine;

public class DamageOnContact2D : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 Goal 태그인지 확인
        if (collision.collider.CompareTag("Goal")|| collision.collider.CompareTag("Enemy"))
        {
            HealthWithUI2D targetHealth = collision.collider.GetComponent<HealthWithUI2D>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log($"Damage applied! Target: {collision.collider.name}, Damage: {damage}, CurrentHealth: {targetHealth.CurrentHealth}");
            }
        }

        // 필요 시 다른 태그 처리 가능
        // else if (collision.collider.CompareTag("Monster")) { ... }
    }
}
