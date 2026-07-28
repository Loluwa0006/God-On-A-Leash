using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : MonoBehaviour
{
    enum HitboxDrawMode
    {
        NoDraw,
        Wireframe,
        Solid
    }

    public const int MAX_CONTACTS_PER_FRAME = 4;
    [SerializeField] Collider hitboxCollider;

    public Collider HitboxCollider
    { 
        get 
        {

            if (isBoxCollider)
            {
                return hitboxCollider as BoxCollider;
            }
            else
            {
                return hitboxCollider as SphereCollider;
            }
                
         }
        
       set => hitboxCollider = value;
    }
    [SerializeField] LayerMask hitboxMask;
    [SerializeField] List<HealthComponent> blacklistedTargets;
    [SerializeField] DamageInfo damageInfo;
    [SerializeField] bool activeOnStart = false;
    public DamageInfo DamageInfo { get { return damageInfo; } set { damageInfo = value; } }

    [Header("Editor")]
    [SerializeField] Color inactiveColor = Color.red;
    [SerializeField] Color activeColor = Color.green;
    [SerializeField] HitboxDrawMode hitboxDrawMode = HitboxDrawMode.NoDraw;

    Collider[] struckTargets = new Collider[MAX_CONTACTS_PER_FRAME];
    HashSet<HealthComponent> previousTargets = new();


    bool isBoxCollider;
    bool wasActive;

    public Action<HashSet<HealthComponent>> targetsStruck;
    [HideInInspector] public bool HitboxActive = false;
    private void Start()
    {   
        if (hitboxCollider == null) hitboxCollider = GetComponent<Collider>();
        isBoxCollider = hitboxCollider is BoxCollider;
        HitboxActive = activeOnStart;
    }
    public void OnActivate()
    {
        previousTargets.Clear();
    }

    public void OnDeactivate()
    {
        targetsStruck?.Invoke(previousTargets);
    }

    private void FixedUpdate()
    {
        if (hitboxCollider.enabled)
        {
            CheckForCollisions();
        }
    }

    private void Update()
    {
        hitboxCollider.enabled = HitboxActive;
        if (!wasActive && hitboxCollider.enabled)
        {
            OnActivate();
        }
        else if (wasActive && !hitboxCollider.enabled)
        {
            OnDeactivate();
        }
        wasActive = hitboxCollider.enabled;
    }
    void CheckForCollisions()
    {
        for (int i = 0; i < struckTargets.Length; i++)
        {
            struckTargets[i] = null;
        }
        if (!isBoxCollider) Physics.OverlapSphereNonAlloc(hitboxCollider.bounds.center, hitboxCollider.bounds.extents.x, struckTargets, hitboxMask, QueryTriggerInteraction.Collide);
        else Physics.OverlapBoxNonAlloc(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, struckTargets, hitboxCollider.transform.rotation, hitboxMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < struckTargets.Length; i++)
        {
            var target = struckTargets[i];
            if (target == null) continue;
            if (target.TryGetComponent(out HealthComponent hp))
            {
                if (blacklistedTargets.Contains(hp) || previousTargets.Contains(hp)) continue;
                DamageEntity(hp);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (hitboxDrawMode == HitboxDrawMode.NoDraw || hitboxCollider == null) return;
        bool isBoxCollider = hitboxCollider is BoxCollider;
        Color hitboxColor;
       if (HitboxActive)
        {
            hitboxColor = activeColor;
        }
       else
        {
            hitboxColor = inactiveColor;
        }
       Gizmos.color = hitboxColor;

       if (isBoxCollider)
        {
           if (hitboxDrawMode == HitboxDrawMode.Wireframe)  Gizmos.DrawWireCube(hitboxCollider.bounds.center, hitboxCollider.bounds.size);
           else  Gizmos.DrawCube(hitboxCollider.bounds.center, hitboxCollider.bounds.size);

        }
        else
        {
           if (hitboxDrawMode == HitboxDrawMode.Wireframe) Gizmos.DrawWireSphere(hitboxCollider.bounds.center, hitboxCollider.bounds.extents.x);
           else Gizmos.DrawSphere(hitboxCollider.bounds.center, hitboxCollider.bounds.extents.x);
        }
    }
    void DamageEntity(HealthComponent healthComponent)
    {
        HitboxContactInfo collisionInfo = new()
        {
            DamageInfo = damageInfo,
            collisionPoint = hitboxCollider.bounds.ClosestPoint(healthComponent.Hurtbox.bounds.center),
            hurtbox = healthComponent.Hurtbox,
            
        };
        healthComponent.Damage(collisionInfo);
        previousTargets.Add(healthComponent);
    }
}
[System.Serializable]
public class DamageInfo
{
    public int damage;
    public int hitstunFrames;
    /// <summary>
    /// For knockback that's determined designer side so attacks can have a specific knockback direction. 
    /// </summary>
    [SerializeField, HideIf(nameof(RequiresKnockbackPower))] public Vector3 knockbackVector;
    /// <summary>
    /// For knockback that's determined programmatically by the collision point and the center of the hurtbox. 
    /// </summary>
    [SerializeField, ShowIf(nameof(RequiresKnockbackPower))] public float knockbackPower;
    public DamageSource damageSource;
    //if true, the knockback direction will be determined by the vector from the collision point to the center of the hurtbox. If false, the knockback direction will be determined by the knockbackVector in DamageInfo.
    public bool useCollisionPointToDetermineKnockbackDirection;

    public bool RequiresKnockbackPower() => useCollisionPointToDetermineKnockbackDirection;

    public Vector3 GetKnockbackVector(Vector3 collisionPoint, Vector3 hurtboxCenter)
    {
        if (useCollisionPointToDetermineKnockbackDirection)
        {
            return (hurtboxCenter - collisionPoint).normalized * knockbackPower;
        }
        else
        {
            return knockbackVector;
        }
    }
}

public struct HitboxContactInfo
{
    public DamageInfo DamageInfo;
    public Vector3 collisionPoint;
    public Collider hurtbox;
}
public enum DamageSource : short
{
    PlayerSlash,
    PlayerDragonslash,
    EnemyWall,
    EnemySmallProjectile,
    EnemyHeavyProjectile,
    AnySource
}

