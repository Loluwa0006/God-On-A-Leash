using NaughtyAttributes;
using UnityEngine;

public class ProjectileVelocityModifier : BaseProjectileModifier
{

    [SerializeField] bool useAuthoredDirection = false;
    [SerializeField] bool resetSpeedOnDeactivation = true;
    [SerializeField, ShowIf (nameof(useAuthoredDirection))] Vector3 authoredDirection = Vector3.zero;
    [SerializeField] float delayBeforeMovement = 0.0f;
    [SerializeField] float moveAcceleration = 8.0f;
    [SerializeField] float moveSpeed = 120.0f;
    [SerializeField] Quaternion rotationOffset;

    Vector3 directionTowardsTarget;

    float delayTracker;

    public bool UsesAuthoredDirection() => useAuthoredDirection;

    private void OnValidate()
    {
        if (useAuthoredDirection)
        {
            authoredDirection = authoredDirection.normalized;
        }
    }
    public override void OnProjectileFired()
    {
        delayTracker = delayBeforeMovement;
        if (delayTracker < 0.001f) OnProjectileMovementStart();
    }

    void OnProjectileMovementStart()
    {
        if (!useAuthoredDirection) directionTowardsTarget = (Projectile.Target.position - Projectile.RigidBody.position).normalized;
        else directionTowardsTarget = authoredDirection;
        Quaternion lookTowardsTarget = Quaternion.LookRotation(directionTowardsTarget);
        Projectile.RigidBody.MoveRotation((lookTowardsTarget * rotationOffset).normalized);
    }

    public override void UpdateModifier()
    {
        if (delayTracker > 0.001f)
        {
            delayTracker = Mathf.MoveTowards(delayTracker, 0.0f, Time.fixedDeltaTime);
            if (delayTracker <= 0.001f)
            {
                OnProjectileMovementStart();
            }
        }
        else
        {
            var forceToAdd = directionTowardsTarget * moveAcceleration;
            var newVelocity = forceToAdd + Projectile.RigidBody.linearVelocity;
            if (newVelocity.magnitude >= moveSpeed)
            {
                var speedNormalized = Projectile.RigidBody.linearVelocity.normalized;
                var extraSpeed = Vector3.Dot(forceToAdd, speedNormalized);
                if (extraSpeed > 0)
                {
                    forceToAdd -= extraSpeed * speedNormalized;
                }
            }

            Projectile.RigidBody.AddForce(forceToAdd, ForceMode.VelocityChange);
        }
    }

    public override void OnProjectileDisabled()
    {
        base.OnProjectileDisabled();
        if (resetSpeedOnDeactivation) Projectile.RigidBody.linearVelocity = Vector3.zero;
    }
}
