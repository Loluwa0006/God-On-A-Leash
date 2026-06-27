using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerAirState
{

    [SerializeField] LayerMask parryMask;
    int durationTracker = 0;

    float startingSpeed = 0.0f;

    RaycastHit parryRaycast;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
        typeof (PlayerRailParryState),
         typeof(PlayerShadowstepState),   
        };
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        startingSpeed = Player.RigidBody.linearVelocity.magnitude;
        Player.entityCollision.AddListener(OnPlayerCollision);
        durationTracker = 0;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].Consume();

        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.ParryProjectileInvulnerability, DamageSource.EnemySmallProjectile, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect);
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
        if (AttemptParry())
        {
            PerformParry(Player.PlayerInput.GetMovementDirection(), parryRaycast.normal);
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
    }

    bool AttemptParry()
    {
        float shapecastSize = Mathf.Lerp(1.0f, 1.0f + Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.RodLengthAdditionalParrySize), Player.RodManager.RodLength / Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxRodRange));
        var shapecast = Physics.BoxCast(Player.Collider.bounds.center, Player.Collider.bounds.extents, Player.RigidBody.linearVelocity.normalized,  out RaycastHit hitinfo, Player.Collider.transform.rotation, shapecastSize, parryMask);
        if (shapecast)
        {
            parryRaycast = hitinfo;
        }
        return shapecast;
    }
    void OnPlayerCollision(Collision collision)
    {
        //PerformParry(Player.PlayerInput.GetMovementDirection(), collision.GetContact(0).normal);
    }
  
    void PerformParry(Vector3 movementDirection, Vector3 normal)
    {
        float bounceVelocity = startingSpeed + (startingSpeed * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ParrySpeedIncrease));
        if (durationTracker > Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ProperParryDuration))
        {
            bounceVelocity *= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PartialParrySpeedPenalty);
        }
        Vector3 velocityReflected = Vector3.Reflect(Player.RigidBody.linearVelocity.normalized, normal).normalized;
        Vector3 movementAccountedForRotation = movementDirection.x * viewCamera.transform.right + movementDirection.y * viewCamera.transform.forward;
        Vector3 velocityRotated = Vector3.Lerp(velocityReflected, movementAccountedForRotation.normalized, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ParryBounceControl));

        Player.RigidBody.linearVelocity = velocityRotated * bounceVelocity;
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Parry);

    }
    public override void Exit()
    {
        base.Exit();
        Player.entityCollision.RemoveListener(OnPlayerCollision);
        Player.HealthComponent.RemoveStatusEffect(StatusEffectID.ParryProjectileInvulnerability);
    }
    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].Buffered && !Player.PlayerGrounded;
    }
}
