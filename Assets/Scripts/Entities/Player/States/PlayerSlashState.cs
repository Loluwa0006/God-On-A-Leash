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
        float rodLengthAsPercent = Player.RodManager.RodLength / Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxRodRange);
        if (slashHitbox.HitboxCollider is SphereCollider sphereHitbox)
        {
            sphereHitbox.radius = Mathf.Lerp(baseHitboxSize, baseHitboxSize * (1 + Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSlashRangeBonusFromRodLength)), rodLengthAsPercent);
        }
        releasedSlashButton = false;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume();
        cancelAllowed = false;
        Player.Animator.speed = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSlashSpeed);
    }

    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAirAcceleration));
        transform.rotation = Player.RigidBody.rotation;
        //use jump gravity to make attacks feel more floaty
        ApplyGravity(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID));
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
        var lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;

        CalculateDamageInfo(
            (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinSlashDamage),
            (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxSlashDamage), 
            Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSpeedToSlashDamageCurve, 0, lateralSpeed),
            lateralSpeed
            );
    }

    public virtual void OnHitboxDeactivation(HashSet<HealthComponent> victims)
    {
        foreach (var victim in victims)
        {
            Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Slash);
        }
    }

    protected void CalculateDamageInfo(int minDamage, int maxDamage, float speedSampled, float lateralSpeed)
    {
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
                Player.RodManager.RodLength += Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSlashRodExtensionSpeed) * Time.fixedDeltaTime;
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
        Player.Animator.speed = 1;
    }

    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Buffered;
    }
}
