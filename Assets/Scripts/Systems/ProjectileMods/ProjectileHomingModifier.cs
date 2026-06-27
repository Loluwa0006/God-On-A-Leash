using NaughtyAttributes;
using UnityEngine;

public class ProjectileHomingModifier : BaseProjectileModifier
{

    [SerializeField] float homingPower = 8.0f;
    [SerializeField] bool hasMaxDistanceForHoming = false;
    [SerializeField, ShowIf(nameof(HasMaxDistanceForHoming))] float maxDistanceForHoming = 1500f;
    [SerializeField, ShowIf(nameof(HasMaxDistanceForHoming))] bool invertHoming = false;
    [SerializeField] Vector3 homingTargetOffset = Vector3.zero;



    public bool HasMaxDistanceForHoming() => hasMaxDistanceForHoming;
    public override void InitializeModifier(BaseProjectile owner)
    {
        base.InitializeModifier(owner);
    }
    public override void UpdateModifier()
    {

        if (hasMaxDistanceForHoming) 
        {
            var distance = Vector3.Distance(Projectile.transform.position, Projectile.Target.position);
            if (distance > maxDistanceForHoming)
            {
                return;
            }
        }
        var currentDirection = Projectile.RigidBody.linearVelocity.normalized;
        var homeTarget = Projectile.Target.position + homingTargetOffset;
        var directionTowardsTarget = (homeTarget - Projectile.RigidBody.position).normalized;


        var directionRotated = Vector3.RotateTowards(currentDirection, directionTowardsTarget, homingPower, 0.001f);

        Projectile.RigidBody.linearVelocity = directionRotated.normalized * Projectile.RigidBody.linearVelocity.magnitude;
    }
}
