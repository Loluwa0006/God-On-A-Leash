using UnityEngine;
using System.Collections.Generic;
public class ProjectileContactModifier : BaseProjectileModifier
{
    const int MAX_CONTACTS_PER_FRAME = 3;

    [SerializeField] LayerMask projectileMask;
    [SerializeField] DamageInfo hitboxInfo;
    [SerializeField] List<HealthComponent> blacklistedTargets = new();
    [SerializeField] Collider hitboxCollider;
    [SerializeField] PostContactLogic postContactLogic;

    Collider[] hitboxResults = new Collider[MAX_CONTACTS_PER_FRAME];

    List<HealthComponent> unallowedTargets = new();
    public enum PostContactLogic
    {
        DisableProjectile,
        DisableHitbox,
    }

    public override void InitializeModifier(BaseProjectile owner)
    {
        base.InitializeModifier(owner);
        if (hitboxCollider == null) hitboxCollider = GetComponent<Collider>();  
    }
    public override void OnProjectileFired()
    {
        base.OnProjectileFired();
        unallowedTargets.Clear();
        for (int i = 0; i < blacklistedTargets.Count; i++)
        {
            unallowedTargets.Add(blacklistedTargets[i]);
        }
    }
    public override void UpdateModifier()
    {
        for (int i = 0; i < MAX_CONTACTS_PER_FRAME; i++)
        {
            hitboxResults[i] = null;
        }
        var overlap = Physics.OverlapSphereNonAlloc(hitboxCollider.bounds.center, hitboxCollider.bounds.extents.z, hitboxResults, projectileMask, QueryTriggerInteraction.Collide);
        if (overlap > 0)
        {
            bool validContact = false;
            for (int i = 0; i < overlap; i++)
            {
                if (DamageHealthComponent(hitboxResults[i], Projectile.ProjectileCollider))
                {
                    validContact = true;
                }
            }
            if (validContact) Projectile.DisableProjectile();
        }
    }
    public bool DamageHealthComponent(Collider hurtbox, Collider hitbox)
    {
        if (!hurtbox.TryGetComponent(out HealthComponent healthComponent)) return false;      
        if (unallowedTargets.Contains(healthComponent)) return false;
        
        HitboxContactInfo contactInfo = new()
        {
            DamageInfo = hitboxInfo,
            hurtbox = healthComponent.Hurtbox,
            collisionPoint = healthComponent.Hurtbox.ClosestPoint(hitbox.bounds.center)
        };
        healthComponent.Damage(contactInfo);
        Projectile.ProjectileLanded.Invoke(healthComponent);
        unallowedTargets.Add(healthComponent);
        return true;
    }
}