using System;
using System.Collections.Generic;

public class PlayerFallState : PlayerAirState
{

    public enum PlayerFallStateMessage
    {
        JumpInfo
    }
    public override Type[] statesToAttemptToTransitionTo 
    {
        get => new Type[]
        {
            typeof(PlayerSlashState),
            typeof(PlayerYawnState),
            typeof(PlayerShadowstepState),
            
            typeof(PlayerParryState),
            typeof(PlayerDashState),
            typeof(PlayerSwingState),
            typeof(PlayerThrowWormState),
            typeof(PlayerJumpState),
            typeof(PlayerRunState),
            typeof(PlayerIdleState),

        };
    }


    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedFalling));
    }

    public override void PhysicsProcess()
    {
        Player.PlayerGrounded = IsGrounded();
        ApplyGravity(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.FALL_GRAVITY_ID));
        
        AirborneMovement(Player.PlayerInput.GetMovementDirection().normalized, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAirAcceleration));
        if (Player.PlayerGrounded)
        {
            if (Player.PlayerInput.GetMovementDirection().magnitude > MOVEMENT_DEADZONE)
            {
                StateMachine.TransitionTo<PlayerRunState>();
            }
            else
            {
                StateMachine.TransitionTo<PlayerIdleState>();
            }
        }
    }

    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedFalling));
    }
    public override bool StateAvailable()
    {
        return !Player.PlayerGrounded;
    }
}
