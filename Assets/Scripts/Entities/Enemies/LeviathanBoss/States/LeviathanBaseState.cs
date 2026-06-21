using UnityEngine;
using UnityEngine.EventSystems;

public class LeviathanBaseState : BaseState
{

    public LeviathanEntity Leviathan { get; set; }


    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        Leviathan = owner.GetComponent<LeviathanEntity>();
    }

    protected Vector3 GetDirectionTowardsTarget()
    {
        return (Leviathan.Target.transform.position - Leviathan.RigidBody.position).normalized;
    }

    protected Vector3 GetDirectionAwayFromTarget()
    {
        return (Leviathan.RigidBody.position - Leviathan.Target.transform.position).normalized;

    }
}
