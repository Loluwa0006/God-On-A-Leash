using UnityEngine;

public class BaseProjectileModifier : MonoBehaviour
{
   [SerializeField] int priority;

    public int Priority { get => priority; private set => priority = value; }
    public BaseProjectile Projectile { get; set; } 
    public virtual void InitializeModifier(BaseProjectile owner)
    {
        Projectile = owner;
    }
    public virtual void UpdateModifier()
    {

    }
    /// <summary>
    /// Called while the projectile is not currently active
    /// </summary>

    public virtual void InactiveUpdateModifier()
    {

    }

    public virtual void OnProjectileFired()
    {

    }

    public virtual void OnProjectileDisabled()
    {

    }

    public virtual void OnProjectileLanded(HealthComponent victim)
    {

    }
}
