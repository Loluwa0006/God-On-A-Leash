using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{

    Dictionary<string, object> fallStateTransitionDictionary = new();

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

    //public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    //{
    //    base.InitializeState(stateMachine, owner);
    //   fallStateTransitionDictionary[PlayerFallState.PlayerFallStateMessage.JumpInfo.ToString()] = Player.StatsManager.GroundedJumpInfo;
    //}

    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.RigidBody.AddForce(Vector3.up * Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.PlayerGroundedJumpPower), ForceMode.VelocityChange);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Jump].Consume();
    }

    public override void PhysicsProcess()
    {
        Player.PlayerGrounded = IsGrounded();
        ApplyGravity( Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.PlayerJumpGravity));
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.PlayerAirAcceleration));
        if (Player.RigidBody.linearVelocity.y <= 0.0f)
        {
            StateMachine.TransitionTo<PlayerFallState>(fallStateTransitionDictionary);
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
