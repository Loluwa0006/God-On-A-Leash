using System.Collections.Generic;
using UnityEngine;

public class LeviathanMoveState : LeviathanBaseState
{

    Vector2 moveDirection;

    int moveDuration;
    public override void Enter(Dictionary<string, object> message = null)
    {
        moveDirection = GetDirectionAwayFromTarget();
        var minMoveDuration = Leviathan.StatsManager.GetValueFromStat(StatDatabase.Instance.LeviathanStats.LeviathanMinMoveDuration);
        var maxMoveDuration = Leviathan.StatsManager.GetValueFromStat(StatDatabase.Instance.LeviathanStats.LeviathanMaxMoveDuration);
        moveDuration = (int) Random.Range(minMoveDuration, maxMoveDuration);
    }
    public override void PhysicsProcess()
    {
        float acceleration = Leviathan.StatsManager.GetValueFromStat(StatDatabase.Instance.LeviathanStats.LeviathanMoveAcceleration);
        Vector2 lateralAddition = new(moveDirection.x * acceleration, moveDirection.y * acceleration);
        Vector2 currentSpeed = Leviathan.RigidBody.linearVelocity;
        if (currentSpeed.magnitude >= Leviathan.StatsManager.GetValueFromStat(StatDatabase.Instance.LeviathanStats.LeviathanMoveSpeed))
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

    public override bool StateAvailable()
    {
        return true;
    }
}
