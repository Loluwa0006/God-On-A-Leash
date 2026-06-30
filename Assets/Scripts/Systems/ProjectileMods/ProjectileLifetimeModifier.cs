using UnityEngine;

public class ProjectileLifetimeModifier : BaseProjectileModifier
{
    [SerializeField] float lifetimeInSeconds = 7.0f;
    int MaxLifeTime => Mathf.FloorToInt(lifetimeInSeconds / Time.fixedDeltaTime);

    int lifetimeTracker = 0;

    public int LifetimeRemaining => Mathf.Max(MaxLifeTime - lifetimeTracker, 0);

    public override void OnProjectileFired()
    {
        base.OnProjectileFired();
        lifetimeTracker = 0;
    }

    public override void UpdateModifier()
    {
        base.UpdateModifier();
        lifetimeTracker++;
        if (LifetimeRemaining == 0)
        {
            Projectile.DisableProjectile();
        }
    }
}
