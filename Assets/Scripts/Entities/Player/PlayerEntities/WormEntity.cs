using UnityEngine;

public class WormEntity : BaseEntity
{
    [Header("Worm Settings")]
    [SerializeField] int gravityFreeTime = 7 * 60;
    [SerializeField] float distanceWhereTargetConsideredReached = 8.0f;
    [SerializeField] float flySpeed = 60.0f;
    [SerializeField] float gravity = 8.0f;
    [SerializeField] float maxFallSpeed = 30.0f;
    [SerializeField] int hitsBeforeDeactivation = 2;

    [Header("References")]
    [SerializeField] GameObject model;
    [SerializeField] Collider swingbox;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] HealthComponent healthComponent;

    Vector3 target;
    int gravityTracker = 0;
    public bool wormActive { get; private set; }
    public Rigidbody Rigidbody { get => rigidBody; }
    bool reachedTarget = false;
    int remainingHitsBeforeDeactivation = 2;
    public void Fire(Vector3 target, Vector3 startingLocation, Vector3 ownerVelocity)
    {
        rigidBody.isKinematic = false;
        this.target = target;
        gravityTracker = gravityFreeTime;
        reachedTarget = false;
        transform.position = startingLocation;
        Vector3 direction = (target - startingLocation).normalized;

        float velocityToInheritFromOwner = Vector3.Dot(direction, ownerVelocity.normalized);

        rigidBody.linearVelocity = direction * flySpeed + ownerVelocity * velocityToInheritFromOwner;

        wormActive = true;
        model.SetActive(true);
        if (swingbox != null) swingbox.enabled = true;
        transform.LookAt(target);

        remainingHitsBeforeDeactivation = hitsBeforeDeactivation;
    }

    public void Deactivate()
    {
        if (swingbox != null) swingbox.enabled = false;
        model.SetActive(false);
        rigidBody.isKinematic = true;
        wormActive = false;
    }

    public override void PhysicsProcess()
    {
        if (!wormActive) return;
        GravityLogic();
        TargetLogic();
    }

    public void OnHurtboxStruck(HitboxContactInfo contactInfo)
    {
        if (contactInfo.DamageInfo.damageSource == DamageSource.PlayerSlash)
        {
            var directionAwayFromContactPoint = (transform.position - contactInfo.collisionPoint).normalized;
            var startingLocation = transform.position;
            var ownerVelocity =  directionAwayFromContactPoint * contactInfo.DamageInfo.horizontalKnockback;
            Fire(target, startingLocation, ownerVelocity);
            remainingHitsBeforeDeactivation--;
            if (remainingHitsBeforeDeactivation <= 0)
            {
                Deactivate();
                remainingHitsBeforeDeactivation = hitsBeforeDeactivation;
            }
        }
    }

    void GravityLogic()
    {
        if (gravityTracker > 0)
        {
            gravityTracker--;
        }
        if (gravityTracker == 0)
        {

            var gravityForce = gravity;
            float differenceBetweenCurrentSpeedAndMaxFallSpeed = Mathf.Abs(rigidBody.linearVelocity.y - maxFallSpeed);
            if (differenceBetweenCurrentSpeedAndMaxFallSpeed < gravityForce)
            {
                gravityForce = differenceBetweenCurrentSpeedAndMaxFallSpeed;
            }
            rigidBody.AddForce(Vector3.down * gravityForce, ForceMode.VelocityChange);
        }
    }

    void TargetLogic()
    {
        if (reachedTarget) return;
        //if (Vector3.Distance(transform.position, target) <= distanceWhereTargetConsideredReached)
        //{
        //    rigidBody.linearVelocity = 
        //    reachedTarget = true;
        //}
    }


}
