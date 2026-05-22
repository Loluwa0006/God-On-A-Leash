using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwingState : PlayerAirState
{
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
        if (Player.RigidBody.linearVelocity.y > 0) gravity = Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SwingRiseGravity);
        else gravity = Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SwingFallGravity);
        if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Buffered)
        {
            PerformSwingJump();
            return;
        }
        ApplyGravity(gravity);
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SwingAcceleration));
    }

    void PerformSwingJump()
    {
        var jumpVelocity = Player.RigidBody.linearVelocity.normalized * (Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SwingJumpPower) + (Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SwingSpeedToJumpPowerRatio) * Player.RigidBody.linearVelocity.magnitude));
        //var jumpVelocity = Player.RigidBody.linearVelocity.normalized * (Player.StatsManager.SwingJumpInfo.JumpVelocity + (Player.StatsManager.SwingSpeedToJumpPowerRatio * Player.RigidBody.linearVelocity.magnitude));
        float minSwingJumpHeight = Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MinSwingJumpHeight);
        if (jumpVelocity.y < minSwingJumpHeight) jumpVelocity.y = minSwingJumpHeight;
        Player.RigidBody.AddForce(jumpVelocity, ForceMode.VelocityChange);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Consume();
        StateMachine.TransitionTo<PlayerFallState>();        
    }
  
    public override void Exit()
    {
        base.Exit();
        Player.RodManager.RetractRod();
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Swing);
        Player.CameraManager.TransitionToCamera(Player.CameraManager.DefaultCamera, cameraTransitionTime);
    }

    public override bool StateAvailable()
    {
        if (GrappleUtilities.AimingAtGrappable(Player, Player.RodManager.GrappleMask) && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Swing].Buffered)
        {
            return true;
        }
        return false;
    }
}

