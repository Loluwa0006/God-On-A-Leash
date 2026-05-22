using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerAirState
{

    [SerializeField] LayerMask parryMask;
    int durationTracker = 0;

    float startingSpeed = 0.0f;

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
//        Player.entityTriggerEntry.AddListener(OnPlayerTriggerEnter);
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
            gravity = Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.JumpGravity);
        }
        else
        {
            gravity = Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.FallGravity);
        }
        ApplyGravity(gravity);
        Vector3 movementDirection = Player.PlayerInput.GetMovementDirection();
        AirborneMovement(movementDirection, Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.ParryStrafeSpeed));
        durationTracker++;
        if (StateMachine.IsStateAvailable<PlayerRailParryState>())
        {
            StateMachine.TransitionTo<PlayerRailParryState>();
            return;
        }
        
        if (durationTracker == Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.ProperParryDuration) + Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.PartialParryDuration))
        {
            StateMachine.TransitionTo<PlayerFallState>();
        }
    }

    void OnPlayerCollision(Collision collision)
    {
        PerformParry(Player.PlayerInput.GetMovementDirection(), collision.GetContact(0).normal);
    }
    //void OnPlayerTriggerEnter(Collider collider)
    //{
    //    if ((parryMask & (1 << collider.gameObject.layer)) != 0)
    //    {
    //        PerformParry(Player.PlayerInput.GetMovementDirection(), Vector3.zero);
    //    }
    //}
    void PerformParry(Vector3 movementDirection, Vector3 normal)
    {
        float bounceVelocity = startingSpeed + (startingSpeed * Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.ParrySpeedIncrease));
        if (durationTracker > Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.ProperParryDuration))
        {
            bounceVelocity *= Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.PartialParrySpeedPenalty);
        }
        Vector3 velocityReflected = Vector3.Reflect(Player.RigidBody.linearVelocity.normalized, normal).normalized;
        Vector3 movementAccountedForRotation = movementDirection.x * viewCamera.transform.right + movementDirection.y * viewCamera.transform.forward;
        Vector3 velocityRotated = Vector3.Lerp(velocityReflected, movementAccountedForRotation.normalized, Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.ParryBounceControl));

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
