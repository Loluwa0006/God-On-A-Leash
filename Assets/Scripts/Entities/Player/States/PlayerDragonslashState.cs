using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDragonslashState : PlayerBaseState
{
    float dragonslashSpeed;
    [HideInInspector] public bool dragonslashAnimationOver = false;
    [SerializeField] HitboxComponent dragonslashHitbox;
    [SerializeField] float cameraTransitionTime = 0.1f;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {

        };
    }

    float baseHitboxSize;
    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        if (dragonslashHitbox == null) dragonslashHitbox = GetComponent<HitboxComponent>();
        dragonslashHitbox.targetsStruck += OnHitboxDeactivation;
        if (dragonslashHitbox.HitboxCollider is SphereCollider sphereHitbox)
        {
            baseHitboxSize = sphereHitbox.radius;
        }
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_InSquashbuckler), true);
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_IsAttacking));
        dragonslashSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        float rodLengthAsPercent = Player.RodManager.RodLength / Player.StatsManager.GetValueFromStat(StatID.PlayerMaxRodRange);
        dragonslashSpeed += Mathf.Lerp(0, Player.StatsManager.GetValueFromStat(StatID.PlayerDragonslashSpeedBonusFromRodLength), 1.0f - rodLengthAsPercent);
        if (dragonslashHitbox.HitboxCollider is SphereCollider sphereHitbox)
        {
            sphereHitbox.radius = Mathf.Lerp(baseHitboxSize, baseHitboxSize * (1 + Player.StatsManager.GetValueFromStat(StatID.PlayerSlashRangeBonusFromRodLength)), rodLengthAsPercent);
        }
        
        dragonslashAnimationOver = false;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume();
        Player.SquashbucklerManager.SquashbucklerCharge = 0;
        Player.CameraManager.TransitionToCamera(Player.CameraManager.CloseFollowCamera, cameraTransitionTime);
    }

    public override void PhysicsProcess()
    {
       
        if (dragonslashAnimationOver)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.RigidBody.linearVelocity = dragonslashSpeed * viewCamera.transform.forward;
        float lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        CalculateDamage(
            (int)Player.StatsManager.GetValueFromStat(StatID.PlayerMinDragonslashDamage),
            (int)Player.StatsManager.GetValueFromStat(StatID.PlayerMaxDragonslashDamage),
            Player.StatsManager.GetValueFromStat(StatID.PlayerSpeedToDragonslashDamageCurve, lateralSpeed)
            );
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

    protected void CalculateDamage(int minDamage, int maxDamage, float speedSampled)
    {
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
        Player.CameraManager.TransitionToCamera(Player.CameraManager.DefaultCamera, cameraTransitionTime);
    }

}

