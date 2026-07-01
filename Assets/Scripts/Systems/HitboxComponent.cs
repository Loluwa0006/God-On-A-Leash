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

    public DamageInfo DamageInfo { get { return damageInfo; } set { damageInfo = value; } }

    [Header("Editor")]
    [SerializeField] Color inactiveColor = Color.red;
    [SerializeField] Color activeColor = Color.green;
    [SerializeField] HitboxDrawMode hitboxDrawMode = HitboxDrawMode.NoDraw;

    Collider[] struckTargets = new Collider[MAX_CONTACTS_PER_FRAME];
    List<HealthComponent> previousTargets = new();


    bool isBoxCollider;
    bool wasActive;

    public Action<List<HealthComponent>> targetsStruck;
    [HideInInspector] public bool HitboxActive = false;
    private void Start()
    {   
        if (hitboxCollider == null) hitboxCollider = GetComponent<Collider>();
        isBoxCollider = hitboxCollider is BoxCollider;
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
        Debug.Log($"Damaging {healthComponent.name} for {damageInfo.damage} damage");
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
public struct DamageInfo
{
    public int damage;
    public float horizontalKnockback;
    public float verticalKnockback;
    public int hitstunFrames;
    public DamageSource damageSource;
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

