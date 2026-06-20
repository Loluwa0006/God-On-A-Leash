using System.Collections.Generic;
using UnityEngine;

public class LeviathanIdleState : LeviathanBaseState
{
    int idleDuration;
    public override void Enter(Dictionary<string, object> message = null)
    {
        var minIdleDuration = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMinIdleDuration);
        var maxIdleDuration = Leviathan.StatsManager.GetValueFromStat(StatID.LeviathanMaxIdleDuration);
        idleDuration = (int) Random.Range(minIdleDuration, maxIdleDuration);
    }
    public override void PhysicsProcess()
    {
        idleDuration--;
        if (idleDuration <= 0)
        {
            StateMachine.TransitionTo<LeviathanMoveState>();
        }
    }
}
