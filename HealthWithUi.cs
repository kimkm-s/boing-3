using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthWithUI2D : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public Action OnDiedCallback;

    [Header("UI Settings")]
    public Slider healthSlider;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        // 자식에서 Slider 자동 감지
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        TemporaryWall wall = GetComponent<TemporaryWall>();
        if (wall != null)
        {
            wall.OnWallDestroyed();
        }

        UpdateHealthUI();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = (float)CurrentHealth / maxHealth;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} Die() 실행됨");

        OnDiedCallback?.Invoke();
    }
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        // ... (UI 업데이트 로직이 있다면 여기에 추가) ...
    }

}
