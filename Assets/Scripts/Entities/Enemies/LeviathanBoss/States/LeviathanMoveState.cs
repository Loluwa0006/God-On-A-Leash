using System.Collections.Generic;
using UnityEngine;

public class LeviathanMoveState : LeviathanBaseState
{

    Vector2 moveDirection;

    int moveDuration;
    public override void Enter(Dictionary<string, object> message = null)
    {
        if (Leviathan.Target == null)
        {
            Debug.Log("Leviathan target is null");
            StateMachine.TransitionTo<LeviathanIdleState>();
            return;
        }
        moveDirection = (Leviathan.RigidBody.position - Leviathan.Target.transform.position).normalized;
        var minMoveDuration = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMinMoveDuration);
        var maxMoveDuration = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMaxMoveDuration);
        moveDuration = (int) Random.Range(minMoveDuration, maxMoveDuration);
    }
    public override void PhysicsProcess()
    {
        float acceleration = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMoveAcceleration);
        Vector2 lateralAddition = new(moveDirection.x * acceleration, moveDirection.y * acceleration);
        Vector2 currentSpeed = Leviathan.RigidBody.linearVelocity;
        if (currentSpeed.magnitude >= Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMoveSpeed))
        {
            var speedNormalized = currentSpeed.normalized;
            var extraSpeed = Vector2.Dot(lateralAddition, speedNormalized);
            if (extraSpeed > 0)
            {
                lateralAddition -= extraSpeed * speedNormalized;
            }
        }

        Leviathan.RigidBody.AddForce(new Vector3(lateralAddition.x, 0, lateralAddition.y), ForceMode.VelocityChange);
        moveDuration--;
        if (moveDuration <= 0)
        {
            StateMachine.TransitionTo<LeviathanIdleState>();
        }
    }

}
