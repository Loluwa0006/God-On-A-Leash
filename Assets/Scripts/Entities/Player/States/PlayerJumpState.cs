using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerSlashState),
            typeof(PlayerParryState),   
            typeof(PlayerDashState),
            typeof(PlayerSwingState),
            typeof(PlayerThrowWormState),
        };
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.RigidBody.AddForce(Vector3.up * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_VELOCITY_ID), ForceMode.VelocityChange);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Consume();
    }

    public override void PhysicsProcess()
    {
        Player.PlayerGrounded = IsGrounded();
        ApplyGravity( Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID));
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAirAcceleration));
        if (Player.RigidBody.linearVelocity.y <= 0.0f)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
    }

    public override bool StateAvailable()
    {
        if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Buffered && Player.PlayerGrounded)
        {
            return true;
        }
        return false;
    }

}
