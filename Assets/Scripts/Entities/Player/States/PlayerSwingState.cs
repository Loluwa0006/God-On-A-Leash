using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerSwingState : PlayerAirState
{
    [SerializeField] MultiAimConstraint swingIKConstraint;
    [SerializeField] Transform swingIKTarget;
    [SerializeField] float cameraTransitionTime = 0.35f;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerSlashState),
            typeof(PlayerShadowstepState),
            
            typeof(PlayerParryState),
            typeof(PlayerThrowWormState),
            typeof(PlayerDashState),
        };
    }


    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.RodManager.StartSwing();
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Swing].Consume();
        Player.CameraManager.TransitionToCamera(Player.CameraManager.WideFollowCamera, cameraTransitionTime);
        swingIKTarget.position = Player.RodManager.GrappleInfo.GrapplePosition;
        swingIKConstraint.weight = 1f;
    }


    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedSwing));
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_JumpPerformed));
    }
    public override void Process()
    {
        
        if (StateMachine.IsStateAvailable<PlayerDashState>())
        {
            StateMachine.TransitionTo<PlayerDashState>();
            return;
        }
        if (!Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Swing].ActionPressed)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        
        AttemptStateTransition();
    }
    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        float gravity;
        if (Player.RigidBody.linearVelocity.y > 0) gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.SwingJumpInfo, JumpInfo.JUMP_GRAVITY_ID);
        else gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerWormJumpInfo, JumpInfo.FALL_GRAVITY_ID);
        if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Buffered)
        {
            PerformSwingJump();
            return;
        }
        ApplyGravity(gravity);
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.SwingAcceleration));
        swingIKTarget.position = Player.RodManager.GrappleInfo.GrapplePosition;
    }

    void PerformSwingJump()
    {
        var jumpVelocity = Player.RigidBody.linearVelocity.normalized * (Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.SwingJumpInfo) + (Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.SwingSpeedToJumpPowerRatio) * Player.RigidBody.linearVelocity.magnitude));
        //var jumpVelocity = Player.RigidBody.linearVelocity.normalized * (Player.StatsManager.SwingJumpInfo.JumpVelocity + (Player.StatsManager.SwingSpeedToJumpPowerRatio * Player.RigidBody.linearVelocity.magnitude));
        float minSwingJumpHeight = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.MinSwingJumpHeight);
        if (jumpVelocity.y < minSwingJumpHeight) jumpVelocity.y = minSwingJumpHeight;
        Player.RigidBody.AddForce(jumpVelocity, ForceMode.VelocityChange);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Consume();
        StateMachine.TransitionTo<PlayerFallState>();
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_JumpPerformed));
    }
  
    public override void Exit()
    {
        base.Exit();
        Player.RodManager.DisableGrapple();
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Swing);
        Player.CameraManager.TransitionToCamera(Player.CameraManager.DefaultCamera, cameraTransitionTime);
        swingIKConstraint.weight = 0f;
    }

    public override bool StateAvailable()
    {
        if (GrappleUtilities.AimingAtGrappable(Player, Player.RodManager.GrappleMask) && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Swing].Buffered && Player.RodManager.RodLength <= 0.001f)
        {
            return true;
        }
        return false;
    }
}

