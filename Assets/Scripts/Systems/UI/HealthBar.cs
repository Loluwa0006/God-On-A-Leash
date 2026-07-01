using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] HealthComponent healthComponent;
    [SerializeField] Slider healthBar;

    private void Start()
    {
        healthBar.value = 1.0f;
    }
    public void OnEntityDamaged(HitboxContactInfo info)
    {
        healthBar.value = healthComponent.Health / healthComponent.MaxHealth;
    }
    public void OnEntityHealed(int amount)
    {
        healthBar.value =  healthComponent.Health / healthComponent.MaxHealth;
    }

    public void OnEntityKilled()
    {
        healthBar.value = 0.0f;
    }
}
