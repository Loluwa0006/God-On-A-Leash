using System;

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

    public override void PhysicsProcess()
    {
        Player.PlayerGrounded = IsGrounded();
        ApplyGravity(Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.FallGravity));
        
        AirborneMovement(Player.PlayerInput.GetMovementDirection().normalized, Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.AirAcceleration));
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

    public override bool StateAvailable()
    {
        return !Player.PlayerGrounded;
    }
}
