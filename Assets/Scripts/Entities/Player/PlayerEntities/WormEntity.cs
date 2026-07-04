using UnityEngine;

public class WormEntity : BaseEntity
{
    [Header("Worm Settings")]
    [SerializeField] int gravityFreeTime = 7 * 60;
    [SerializeField] float flySpeed = 60.0f;
    [SerializeField] float gravity = 8.0f;
    [SerializeField] float maxFallSpeed = 30.0f;
    [SerializeField] int hitsBeforeDeactivation = 2;

    [Header("References")]
    [SerializeField] GameObject model;
    [SerializeField] Collider swingbox;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] HealthComponent healthComponent;

    int gravityTracker = 0;
    public bool wormActive { get; private set; }
    public Rigidbody Rigidbody { get => rigidBody; }
    int remainingHitsBeforeDeactivation = 2;

    public override void Initialize()
    {
        base.Initialize();
        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.WormPlayerSlashInvulnerability, DamageSource.PlayerSlash, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        healthComponent.AddStatusEffect(invulnerabilityEffect);
    }
    public void Fire(Vector3 direction, Vector3 startingLocation, Vector3 ownerVelocity)
    {
        rigidBody.isKinematic = false;
        gravityTracker = gravityFreeTime;
        rigidBody.Move(startingLocation, Quaternion.LookRotation(direction));
        direction = direction.normalized;
        float velocityToInheritFromOwner = Vector3.Dot(direction, ownerVelocity.normalized);

        rigidBody.linearVelocity = direction * flySpeed + ownerVelocity * velocityToInheritFromOwner;

        wormActive = true;
        model.SetActive(true);
        if (swingbox != null) swingbox.enabled = true;

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
    }

    public void OnHurtboxStruck(HitboxContactInfo contactInfo)
    {
        Debug.Log("Worm hit by " + contactInfo.DamageInfo.damageSource);
        if (contactInfo.DamageInfo.damageSource == DamageSource.PlayerSlash)
        {
            remainingHitsBeforeDeactivation--;
            if (remainingHitsBeforeDeactivation <= 0)
            {
                Deactivate();
                return;
            }
            Debug.Log("Worm hit by player slash");
            var directionAwayFromContactPoint = (transform.position - contactInfo.collisionPoint).normalized;
            var startingLocation = transform.position;
            var ownerVelocity =  directionAwayFromContactPoint * contactInfo.DamageInfo.horizontalKnockback;
            Fire(directionAwayFromContactPoint, startingLocation, ownerVelocity);
            
        }
    }

    void GravityLogic()
    {
        gravityTracker = (int) Mathf.MoveTowards(gravityTracker, 0, 1);
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


}
