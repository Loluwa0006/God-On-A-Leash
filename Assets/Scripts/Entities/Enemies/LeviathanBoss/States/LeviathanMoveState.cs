using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeviathanMoveState : LeviathanBaseState
{

    Vector2 moveDirection;

    public override void Enter(Dictionary<string, object> message = null)
    {
        moveDirection = (Leviathan.Target.transform.position - Leviathan.RigidBody.position).normalized;
    }
    public override void PhysicsProcess()
    {
        Vector2 lateralAddition = new(moveDirection.x * acceleration, moveDirection.z * acceleration);
        Vector2 currentSpeed = Leviathan.RigidBody.linearVelocity;
        if (currentSpeed.magnitude >= Leviathan.StatsManager.GetValueFromStat(StatID.PlayerMoveSpeed))
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

}
