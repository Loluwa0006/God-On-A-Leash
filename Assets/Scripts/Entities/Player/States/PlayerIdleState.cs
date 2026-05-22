

using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedMovementState
{
    public override System.Type[] statesToAttemptToTransitionTo
    {
        get => new System.Type[]
        {
            typeof(PlayerDashState),
            typeof(PlayerSwingState),
            typeof(PlayerThrowWormState),
            typeof(PlayerJumpState),
            typeof(PlayerRunState),
        };
    }
    protected override void GroundedMovement()
    {
        Player.PlayerGrounded = IsGrounded();
        if (!Player.PlayerGrounded)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.RigidBody.linearVelocity = Vector3.MoveTowards(Player.RigidBody.linearVelocity, Vector3.zero, Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.DecelerationDrag));
    }
    public override bool StateAvailable()
    {
        Vector2 movementDirection = Player.PlayerInput.GetMovementDirection();
        return Player.PlayerGrounded && movementDirection.magnitude <= MOVEMENT_DEADZONE;
    }

}