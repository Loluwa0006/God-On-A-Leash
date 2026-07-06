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

    public void Initialize(EntityStatsManager statsManager)
    {
        base.Initialize();
        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.WormPlayerSlashInvulnerability, DamageSource.PlayerSlash, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        healthComponent.AddStatusEffect(invulnerabilityEffect);
        StatsManager = statsManager;
    }

    public void Fire(Vector3 direction, Vector3 startingLocation, Vector3 ownerVelocity)
    {
        rigidBody.isKinematic = false;
        gravityTracker = (int) StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormGravityFreeTime);
        rigidBody.Move(startingLocation, Quaternion.LookRotation(direction));
        direction = direction.normalized;
        float velocityToInheritFromOwner = Vector3.Dot(direction, ownerVelocity.normalized);

        rigidBody.linearVelocity = direction * StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormFlySpeed) + ownerVelocity * velocityToInheritFromOwner;

        wormActive = true;
        model.SetActive(true);
        if (swingbox != null) swingbox.enabled = true;

        remainingHitsBeforeDeactivation = (int) StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormHitsBeforeDeactivation);
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
        if (contactInfo.DamageInfo.damageSource == DamageSource.PlayerSlash)
        {
            remainingHitsBeforeDeactivation--;
            if (remainingHitsBeforeDeactivation <= 0)
            {
                Deactivate();
                return;
            }
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

            var gravityForce = StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormGravity);
            float differenceBetweenCurrentSpeedAndMaxFallSpeed = Mathf.Abs(rigidBody.linearVelocity.y - StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormMaxFallSpeed));
            if (differenceBetweenCurrentSpeedAndMaxFallSpeed < gravityForce)
            {
                gravityForce = differenceBetweenCurrentSpeedAndMaxFallSpeed;
            }
            rigidBody.AddForce(Vector3.down * gravityForce, ForceMode.VelocityChange);
        }
    }


}
