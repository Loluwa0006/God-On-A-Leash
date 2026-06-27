using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedMovementState : PlayerBaseState
{


    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {

            typeof(PlayerSwingState),
            typeof(PlayerThrowWormState),
            typeof(PlayerJumpState),
            typeof(PlayerRunState),
            typeof(PlayerIdleState),
        };
    }
 
    protected virtual void GroundedMovement()
    {
        if (!Player.PlayerGrounded)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Vector2 movementDirection = Player.PlayerInput.GetMovementDirection();
        movementDirection = movementDirection.normalized;
        Vector2 currentSpeed =  new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z);
    
        Vector3 moveDirection = movementDirection.x * viewCamera.transform.right + movementDirection.y * viewCamera.transform.forward;
        Vector2 lateralAddition = new Vector2(moveDirection.x * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundAcceleration), moveDirection.z * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundAcceleration));
        

        if (currentSpeed.magnitude >= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMoveSpeed))
        {
            var speedNormalized = currentSpeed.normalized;
            var extraSpeed = Vector2.Dot(lateralAddition, speedNormalized);
            if (extraSpeed > 0)
            {
                lateralAddition -= extraSpeed * speedNormalized;
            }
        }

        Player.RigidBody.AddForce(new Vector3(lateralAddition.x, 0, lateralAddition.y), ForceMode.VelocityChange);
    }
    public override void PhysicsProcess()
    {
        Player.PlayerGrounded = IsGrounded();
        GroundedMovement();
    }


}
