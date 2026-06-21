using System.Collections.Generic;
using UnityEngine;

public class LeviathanIdleState : LeviathanBaseState
{
    int idleDuration;
    public override void Enter(Dictionary<string, object> message = null)
    {
        int minIdleDuration = Mathf.RoundToInt(Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMinIdleDuration));
        int maxIdleDuration = Mathf.RoundToInt(Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMaxIdleDuration));
        idleDuration =  Random.Range(minIdleDuration, maxIdleDuration);
    }
    public override void PhysicsProcess()
    {
        Leviathan.RigidBody.linearVelocity = Vector3.MoveTowards(Leviathan.RigidBody.linearVelocity, Vector3.zero, Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanDecelerationRate));
        idleDuration--;
        if (idleDuration <= 0)
        {
            if (StateMachine.IsStateAvailable<LeviathanLargeBeamState>())
            {
                StateMachine.TransitionTo<LeviathanLargeBeamState>();
                return;
            }
            StateMachine.TransitionTo<LeviathanMoveState>();
        }
    }

    public override bool StateAvailable()
    {
        return true;
    }
}
