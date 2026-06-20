using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LeviathanIdleState : LeviathanBaseState
{
   
    int moveDuration;
    public override void Enter(Dictionary<string, object> message = null)
    {
        moveDuration = UnityEngine.Random.Range(Leviathan.LeviathanStats.MinMoveDuration, Leviathan.LeviathanStats.MaxMoveDuration);
    }
}
