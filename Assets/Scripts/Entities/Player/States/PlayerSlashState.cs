using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSlashState : PlayerAirState
{
    [SerializeField] HitboxComponent slashHitbox;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {

        };
    }

    bool releasedSlashButton = true;
    bool cancelAllowed = false;
    float baseHitboxSize;
    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        if (slashHitbox == null) slashHitbox = GetComponent<HitboxComponent>();
        slashHitbox.targetsStruck += OnHitboxDeactivation;
        if (slashHitbox.HitboxCollider is SphereCollider sphereHitbox)
        {
            baseHitboxSize = sphereHitbox.radius;
        }
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_InSquashbuckler).ToString(), false);
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_IsAttacking).ToString());
        float rodLengthAsPercent = Player.RodManager.RodLength / Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxRodRange);
        if (slashHitbox.HitboxCollider is SphereCollider sphereHitbox)
        {
            sphereHitbox.radius = Mathf.Lerp(baseHitboxSize, baseHitboxSize * (1 + Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SlashRangeBonusFromRodLength)), rodLengthAsPercent);
        }
        releasedSlashButton = false;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume();
        cancelAllowed = false;
    }

    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.AirAcceleration));
        transform.rotation = Player.RigidBody.rotation;
        //use jump gravity to make attacks feel more floaty
        ApplyGravity(Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.JumpGravity));
        if (Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        if (StateMachine.IsStateAvailable<PlayerShadowstepState>())
        {
            StateMachine.TransitionTo<PlayerShadowstepState>();
            return;
        }
        if (cancelAllowed)
        {
            if (StateMachine.IsStateAvailable<PlayerDashState>())
            {
                StateMachine.TransitionTo<PlayerDashState>();
                return;
            }
            if (StateMachine.IsStateAvailable<PlayerSwingState>())
            {
                StateMachine.TransitionTo<PlayerSwingState>();
                return;
            }
            if (StateMachine.IsStateAvailable<PlayerYawnState>())
            {
                StateMachine.TransitionTo<PlayerYawnState>();
                return;
            }
            if (StateMachine.IsStateAvailable<PlayerThrowWormState>())
            {
                StateMachine.TransitionTo<PlayerThrowWormState>();
                return;
            }
        }
        CalculateDamageInfo((int)Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MinSlashDamage), (int)Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxSlashDamage), Player.StatsManager.BaseStats.SpeedToSlashDamageCurve);
    }

    public virtual void OnHitboxDeactivation(List<HealthComponent> victims)
    {
        for (int i = 0; i < victims.Count; i++)
        {
            Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Slash);
        }
    }

    protected void CalculateDamageInfo(int minDamage, int maxDamage, AnimationCurve curve)
    {
        var lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        var speedSampled = curve.Evaluate(lateralSpeed);

        var info = slashHitbox.DamageInfo;
        info.damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, speedSampled));
        info.horizontalKnockback = lateralSpeed;
        slashHitbox.DamageInfo = info;
    }

    public override void InactivePhysicsProcess()
    {
        base.InactivePhysicsProcess();
        if (!releasedSlashButton)
        {
            if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].ActionPressed)
            {
                Player.RodManager.RodLength += Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SlashRodExtensionSpeed) * Time.fixedDeltaTime;
            }
            else
            {
                releasedSlashButton = true;
            }
        }
    }

    public override void Process()
    {
        //no state cancelling
    }
    public override void Exit()
    {
        base.Exit();
        slashHitbox.OnDeactivate();
    }

    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Buffered;
    }
}
