using UnityEngine;
using System.Collections.Generic;
public class ProjectileContactModifier : BaseProjectileModifier
{
    const int MAX_CONTACTS_PER_FRAME = 3;

    [SerializeField] LayerMask projectileMask;
    [SerializeField] DamageInfo hitboxInfo;
    [SerializeField] List<HealthComponent> blacklistedTargets = new();
    [SerializeField] PostContactLogic postContactLogic;

    Collider[] hitboxResults = new Collider[MAX_CONTACTS_PER_FRAME];

    HashSet<HealthComponent> unallowedTargets = new();
    public enum PostContactLogic
    {
        DisableProjectile,
        DisableHitbox,
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
        bool validContact = false;
        for (int x = 0; x < Projectile.ProjectileColliders.Count; x++)
        {
            for (int i = 0; i < MAX_CONTACTS_PER_FRAME; i++)
            {
                hitboxResults[i] = null;
            }
            var hitbox = Projectile.ProjectileColliders[x];
            var overlap = Physics.OverlapSphereNonAlloc(hitbox.bounds.center, hitbox.bounds.extents.magnitude, hitboxResults, projectileMask, QueryTriggerInteraction.Collide);
            for (int y = 0; y < overlap; y++)
            {
                if (DamageHealthComponent(hitboxResults[y], Projectile.ProjectileColliders[x]))
                {
                    validContact = true;
                }
            }
        }
        if (validContact) Projectile.DisableProjectile();
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