using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerParryState : PlayerAirState
{

    public const float PARRY_TERRAIN_RAYCAST_SAFE_MARGIN = 8f;
    // Saved as a const not a float because this is a technical problem not a design one.
    //if you were moving slower then this, then you might have not been moving at all.
    public const float MINIMUM_SPEED_FOR_PARRY = 0.1f;

    [SerializeField] LayerMask parryMask;
    [SerializeField] UnityEvent parryPerformed;
    int durationTracker = 0;

    RaycastHit parryRaycast;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
        typeof (PlayerRailParryState),
         typeof(PlayerShadowstepState),   
        };
    }

    public struct ParryData
    {
        public Vector3 previousSpeed;
        public Vector3 previousLocation;
        public ParryData(Vector3 previousSpeed, Vector3 previousLocation)
        {
            this.previousSpeed = previousSpeed;
            this.previousLocation = previousLocation;
        }
    }

    ParryData parryData;

    public readonly DamageSource[] ParriableSources = { DamageSource.EnemySmallProjectile, DamageSource.Water, DamageSource.EnemyWall };

    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        durationTracker = 0;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].Consume();

        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.ParryProjectileInvulnerability, DamageSource.EnemySmallProjectile, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        InvulnerabilityEffect invulnerabilityEffect2 = new(StatusEffectID.ParryWaterInvulnerability, DamageSource.Water, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect2);
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsParrying), true);
    }
    public override void PhysicsProcess()
    {
        float gravity;
        if (Player.RigidBody.linearVelocity.y > 0)
        {
            gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID);
        }
        else
        {
            gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.FALL_GRAVITY_ID);
        }
        ApplyGravity(gravity);
        Vector3 movementDirection = Player.PlayerInput.GetMovementDirection();
        AirborneMovement(movementDirection, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ParryStrafeSpeed));
        durationTracker++;
        if (StateMachine.IsStateAvailable<PlayerRailParryState>())
        {
            StateMachine.TransitionTo<PlayerRailParryState>();
            return;
        }
        
        if (durationTracker == Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ProperParryDuration) + Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PartialParryDuration))
        {
            StateMachine.TransitionTo<PlayerFallState>();
        }
        if (ParryPossible())
        {
            PerformParry(Player.PlayerInput.GetMovementDirection(), parryRaycast.normal);
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        parryData = new ParryData(Player.RigidBody.linearVelocity, Player.RigidBody.position);
    }

    bool ParryPossible()
    {
        if (parryData.previousSpeed.magnitude <= MINIMUM_SPEED_FOR_PARRY) return false;
        var ray = new Ray(parryData.previousLocation, Player.RigidBody.position - parryData.previousLocation);
        float maxBonusParryRange = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.RodLengthAdditionalParrySize);
        //lerp unclamped so you get the bonus of a rod length stat boost even if you have a rod that is longer than the max length of the rod.
        float bonusParryRange = Mathf.LerpUnclamped(0, maxBonusParryRange, Player.RodManager.RodLengthPercentage);
        // needs to extend past the collider otherwise it will never hit anything due to the origin being the center of the collider in the previous frame.
        bonusParryRange += Player.Collider.bounds.size.magnitude;
        bool collision = Physics.Raycast(ray, out parryRaycast, (Player.RigidBody.position - parryData.previousLocation).magnitude + PARRY_TERRAIN_RAYCAST_SAFE_MARGIN + bonusParryRange, parryMask, QueryTriggerInteraction.Collide);
        return collision;
    }

    public override void OnPlayerStruck(HitboxContactInfo info)
    {
        bool containsSource = false;
        for (int i = 0; i < 3; i++)
        {
            if (info.DamageInfo.damageSource == ParriableSources[i])
            {
                containsSource = true;
                break;
            }
        }
        if (containsSource)
        {
            PerformParry(Player.PlayerInput.GetMovementDirection(), (info.collisionPoint - Player.RigidBody.position).normalized);
            StateMachine.TransitionTo<PlayerFallState>();
        }
        else
        {
            base.OnPlayerStruck(info);
        }
    }

   
    void PerformParry(Vector3 movementDirection, Vector3 normal)
    {
        float previousSpeed = parryData.previousSpeed.magnitude;
        Vector3 previousDirection = parryData.previousSpeed.normalized;
        float bounceVelocity = previousSpeed + (previousSpeed * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ParrySpeedIncrease));
        var hitstopDuration = (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSuccessfulParryHitstopDuration);
        if (durationTracker > Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ProperParryDuration))
        {
            bounceVelocity *= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PartialParrySpeedPenalty);
            hitstopDuration = 0;
        } 
        else
        {
            EntityManager.Instance.SetTimeScale(0, Player, hitstopDuration);
        }
        Vector3 velocityReflected = Vector3.Reflect(previousDirection, normal).normalized;
        Vector3 movementAccountedForRotation = movementDirection.x * viewCamera.transform.right + movementDirection.y * viewCamera.transform.forward;
        Vector3 velocityRotated = Vector3.Lerp(velocityReflected, movementAccountedForRotation.normalized, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ParryBounceControl));

        Player.RigidBody.linearVelocity = velocityRotated * bounceVelocity;
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Parry);
        Vector2 lateralSpeed = new (Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z);
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_AtHighSpeed), lateralSpeed.magnitude >= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSpeedToBeConsideredFast));
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_ParryPerformed));
        parryPerformed.Invoke();
    }
    public override void Exit()
    {
        base.Exit();
        Player.HealthComponent.RemoveStatusEffect(StatusEffectID.ParryWaterInvulnerability);
        Player.HealthComponent.RemoveStatusEffect(StatusEffectID.ParryProjectileInvulnerability);


        // temporary invuln to guarantee that player isn't hit because of hitstop.

        var hitstopDuration = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSuccessfulParryHitstopDuration);

        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.ParryProjectileInvulnerability, DamageSource.EnemySmallProjectile, hitstopDuration + 1);
        InvulnerabilityEffect invulnerabilityEffect2 = new(StatusEffectID.ParryWaterInvulnerability, DamageSource.Water, hitstopDuration + 1);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect2);

    }

    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsParrying), false);
    }
    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].Buffered && !Player.PlayerGrounded;
    }
}
