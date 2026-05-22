using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSlashState : PlayerAirState
{
    [SerializeField] HitboxComponent slashHitbox;

    [HideInInspector] public bool slashAnimationOver = false;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {

        };
    }

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        if (slashHitbox == null) slashHitbox = GetComponent<HitboxComponent>();
        slashHitbox.targetsStruck += OnHitboxDeactivation;
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_IsAttacking).ToString());
        slashAnimationOver = false;
        Player.RigidBody.MoveRotation(Quaternion.LookRotation(viewCamera.transform.forward));
    }

    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.AirAcceleration));
        //use jump gravity to make attacks feel more floaty
        ApplyGravity(Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.JumpGravity));
        if (slashAnimationOver)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        if (StateMachine.IsStateAvailable<PlayerShadowstepState>())
        {
            StateMachine.TransitionTo<PlayerShadowstepState>();
            return;
        }
        CalculateDamage((int)Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MinSlashDamage), (int)Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxSlashDamage), Player.StatsManager.BaseStats.SpeedToSlashDamageCurve);
    }

    public virtual void OnHitboxDeactivation(List<HealthComponent> victims)
    {
        for (int i = 0; i < victims.Count; i++)
        {
            Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Slash);
        }
    }

    protected void CalculateDamage(int minDamage, int maxDamage, AnimationCurve curve)
    {
        var lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        var speedSampled = curve.Evaluate(lateralSpeed);

        var info = slashHitbox.DamageInfo;
        info.damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, speedSampled));
        slashHitbox.DamageInfo = info;
    }

    public override void Process()
    {
        //no state cancelling
    }
    public override void Exit()
    {
        base.Exit();
        Player.CameraManager.ControlPlayerRotation = true;
        slashHitbox.OnDeactivate();
    }

    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Buffered;
    }
}
