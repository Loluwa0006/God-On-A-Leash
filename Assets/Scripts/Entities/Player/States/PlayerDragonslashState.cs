using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDragonslashState : PlayerBaseState
{
    float initialSpeed;
    [HideInInspector] public bool dragonslashAnimationOver = false;
    [SerializeField] HitboxComponent dragonslashHitbox;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {

        };
    }

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        if (dragonslashHitbox == null) dragonslashHitbox = GetComponent<HitboxComponent>();
        dragonslashHitbox.targetsStruck += OnHitboxDeactivation;
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_InSquashbuckler), true);
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_IsAttacking));
        initialSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        dragonslashAnimationOver = false;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume();
        Player.SquashbucklerManager.SquashbucklerCharge = 0;
    }

    public override void PhysicsProcess()
    {
       
        if (dragonslashAnimationOver)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.RigidBody.linearVelocity = initialSpeed * viewCamera.transform.forward;
        CalculateDamage((int) Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MinDragonslashDamage),(int)Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MaxDragonslashDamage), Player.StatsManager.BaseStats.SpeedToDragonslashDamageCurve);
    }
    public override void Process()
    {
        //no automatic state transitions
    }

    public void OnHitboxDeactivation(List<HealthComponent> victims)
    {
        for (int i = 0; i < victims.Count; i++)
        {
            Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Dragonslash);
        }
    }

    protected void CalculateDamage(int minDamage, int maxDamage, AnimationCurve curve)
    {
        var lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        var speedSampled = curve.Evaluate(lateralSpeed);

        var info = dragonslashHitbox.DamageInfo;
        info.damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, speedSampled));
        dragonslashHitbox.DamageInfo = info;
    }

    public override bool StateAvailable()
    {
        return Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Buffered;
    }

    public override void Exit()
    {
        base.Exit();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_InSquashbuckler), false);
    }

}

