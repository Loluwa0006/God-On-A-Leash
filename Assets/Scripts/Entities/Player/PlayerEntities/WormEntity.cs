using UnityEngine;

public class WormEntity : BaseEntity
{
    public EntityStatsManager StatsManager { get; set; }


    [Header("References")]
    [SerializeField] GameObject model;
    [SerializeField] Collider swingbox;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] HealthComponent healthComponent;

    int gravityTracker = 0;
    public bool wormActive { get; private set; }
    public Rigidbody Rigidbody { get => rigidBody; }
    int remainingHitsBeforeDeactivation = 2;

    Vector3 startingLocation;

    float additionalDistanceToTarget = 0; //Used to make the worm fly further if the player is moving really fast.

    bool inFlight = false;
    public void Initialize(EntityStatsManager statsManager)
    {
        base.Initialize();
        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.WormPlayerSlashInvulnerability, DamageSource.PlayerSlash, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        healthComponent.AddStatusEffect(invulnerabilityEffect);
        StatsManager = statsManager;
    }

    public void Fire(Vector3 direction, Vector3 startingLocation, Vector3 ownerVelocity, bool refired = false)
    {
        rigidBody.isKinematic = false;
        gravityTracker = (int) StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormGravityFreeTime);
        rigidBody.Move(startingLocation, Quaternion.LookRotation(direction));
        direction = direction.normalized;
        //Change the range from -1,1 to 0,1 because the worm should never move slower then fire direction
        float velocityToInheritFromOwner = (Vector3.Dot(direction, ownerVelocity.normalized) + 1) / 2.0f;

        rigidBody.linearVelocity = direction * StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormFlySpeed) + ownerVelocity * velocityToInheritFromOwner;

        wormActive = true;
        model.SetActive(true);
        if (swingbox != null) swingbox.enabled = true;

        if (!refired) remainingHitsBeforeDeactivation = (int) StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormHitsBeforeDeactivation);

        inFlight = true;
        this.startingLocation = startingLocation;
        additionalDistanceToTarget = ownerVelocity.magnitude;
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
            if (remainingHitsBeforeDeactivation <= 0)
            {
                Deactivate();
                return;
            }
            remainingHitsBeforeDeactivation--;

            var startingLocation = rigidBody.position;
            var knockbackDirection = contactInfo.DamageInfo.knockbackVector.normalized;
            //knockback power is determined by owner velocity, we multiply by direction to make it a vector for the fire function.
            var ownerVelocity = contactInfo.DamageInfo.knockbackPower * knockbackDirection;
            Fire(knockbackDirection, startingLocation, ownerVelocity, refired: true);
            
        }
    }

    void GravityLogic()
    {
        gravityTracker = (int) Mathf.MoveTowards(gravityTracker, 0, 1);
        if (gravityTracker == 0)
        {

            var gravityForce = StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormGravity);
            float differenceBetweenCurrentSpeedAndMaxFallSpeed = Mathf.Abs(rigidBody.linearVelocity.y - StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormMaxFallSpeed));
            if (differenceBetweenCurrentSpeedAndMaxFallSpeed < gravityForce)
            {
                gravityForce = differenceBetweenCurrentSpeedAndMaxFallSpeed;
            }
            rigidBody.AddForce(Vector3.down * gravityForce, ForceMode.VelocityChange);
        }
    }

    void TargetLogic()
    {
        if (!inFlight) return;
        if (Vector3.Distance(rigidBody.position, startingLocation) >= StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormThrowRange) + additionalDistanceToTarget)
        {
            rigidBody.linearVelocity = Vector3.zero; //Make it hover at the target location instead of falling down.
            inFlight = false;
        }
    }
}
