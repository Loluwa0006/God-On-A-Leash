using UnityEngine;

public class LeviathanBaseState : BaseState
{

    public LeviathanEntity Leviathan { get; set; }


    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        Leviathan = owner.GetComponent<LeviathanEntity>();
    }
}
