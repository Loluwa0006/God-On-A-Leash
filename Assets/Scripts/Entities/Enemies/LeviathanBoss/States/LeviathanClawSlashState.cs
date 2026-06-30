using System.Collections.Generic;
using UnityEngine;

public class LeviathanClawSlashState : LeviathanProjectileState
{
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        var dashImpulse = GetDirectionTowardsTarget() * Leviathan.StatsManager.GetValueFromStat(StatDatabase.Instance.LeviathanStats.LeviathanClawAttackLungeDistance);
        Leviathan.RigidBody.AddForce(dashImpulse, ForceMode.VelocityChange);
    }
}
