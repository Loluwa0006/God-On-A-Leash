using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerYawnState : PlayerAirState
{

    int justYawnTracker = 0;

    int elaspedYawnTime;
    int minYawnTime;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerShadowstepState),
        };
    }

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        Player.AnarchyManager.anarchyGainedThroughScaledMethod.AddListener((method, charges) => OnAnarchyGenerated());
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        if (justYawnTracker > 0)
        {
            OnJustYawn();
        }
        else
        {
            minYawnTime = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinYawnTime);
        }
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn].Consume();
        elaspedYawnTime = 0;
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsYawning), true);
    }
    void OnJustYawn()
    {
        minYawnTime = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinJustYawnTime);
        Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.JustYawn);
        Player.RodManager.RodLength = 0.0f;
    }

    public override void PhysicsProcess()
    {
        elaspedYawnTime++;
        if (elaspedYawnTime >= minYawnTime  && !Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn].ActionPressed) 
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Yawn);
        float gravity;
        if (Player.RigidBody.linearVelocity.y > 0) gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID);
        else gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.FALL_GRAVITY_ID);
        ApplyGravity(gravity);
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerYawnAirAcceleration));
        Player.RodManager.RodLength = Mathf.MoveTowards(Player.RodManager.RodLength, 0.0f, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodRetractionSpeedWhileYawning));
    }

    public override void InactivePhysicsProcess()
    {
        justYawnTracker = (int) Mathf.MoveTowards(justYawnTracker, 0, 1);     
    }

    public override void Exit()
    {
        base.Exit();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsYawning), false);
    }
    public override bool StateAvailable()
    {
        return !Player.PlayerGrounded && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn].Buffered;
    }
    void OnAnarchyGenerated()
    {
        justYawnTracker = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerJustYawnWindow);
        Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerJustYawnWindow);
    }

}
