using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Mod that handles the rendering and collision of beam attacks
/// </summary>
[RequireComponent(typeof(ProjectileContactModifier))]
public class ProjectileBeamMod : BaseProjectileModifier
{
    [SerializeField]  LineRenderer lineRenderer;
    [SerializeField] ProjectileContactModifier contactModifier;
    [SerializeField] SplineContainer beamSpline;
    [SerializeField] SplineCollider.SplineCollider splineCollider;
    [SerializeField] GameObject colliderHolder;
    BezierKnot knotOne = new();
    BezierKnot knotTwo = new();

    
    public override void InitializeModifier(BaseProjectile owner)
    {
        base.InitializeModifier(owner);
        PreventNullComponents();
        knotOne.Position = Vector3.zero;
        knotTwo.Position = Vector3.zero;
        beamSpline.Spline.SetKnot(0, knotOne);
        beamSpline.Spline.SetKnot(1, knotTwo);
        lineRenderer.positionCount = 2;
        splineCollider.OnTriggerEnter += OnBeamCollision;
    }
    void OnBeamCollision(Collider hurtbox)
    {
        var collidersInColliderHolder = colliderHolder.GetComponentsInChildren<Collider>();
        Collider colliderToUse = collidersInColliderHolder[0];
        float currentDistance = Vector3.Distance(colliderToUse.bounds.center, hurtbox.bounds.center);
        for (int i = 1; i < collidersInColliderHolder.Length; i++)
        {
            float newDistance = Vector3.Distance(collidersInColliderHolder[i].bounds.center, hurtbox.bounds.center);
            if (newDistance < currentDistance)
            {
                colliderToUse = collidersInColliderHolder[i];
                currentDistance = newDistance;
            }
        }
        contactModifier.DamageHealthComponent(hurtbox, colliderToUse);
    }
    void PreventNullComponents()
    {
        if (lineRenderer == null) lineRenderer = Projectile.GetComponent<LineRenderer>();
        if (contactModifier == null) contactModifier = Projectile.GetModifier<ProjectileContactModifier>();
        if (beamSpline == null) beamSpline = Projectile.GetComponent<SplineContainer>();
        if (splineCollider == null) splineCollider = Projectile.GetComponent<SplineCollider.SplineCollider>();
    }
    public override void OnProjectileFired()
    {
        base.OnProjectileFired();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Projectile.RigidBody.position);
    }

    public override void UpdateModifier()
    {
        lineRenderer.SetPosition(1, Projectile.RigidBody.position);
        knotTwo.Position = Projectile.RigidBody.position - lineRenderer.GetPosition(0);
        splineCollider.Bake(); 
    }

    public override void OnProjectileDestroyed()
    {
        base.OnProjectileDestroyed();
        lineRenderer.enabled = false;
        splineCollider.ClearBakedSegments();
    }

    private void OnDestroy()
    {
        splineCollider.OnTriggerEnter -= OnBeamCollision;
    }
}
