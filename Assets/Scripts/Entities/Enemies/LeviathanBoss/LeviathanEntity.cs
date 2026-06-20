using UnityEngine;

public class LeviathanEntity : BaseEnemy
{
    [SerializeField] LeviathanStats leviathanStats;
    public LeviathanStats LeviathanStats { get => leviathanStats; }

    [SerializeField] EntityStateMachine stateMachine;

    public EntityStateMachine StateMachine { get => stateMachine; }

   
    public override void Process()
    {
        base.Process();
        stateMachine.Process();
    }

    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        stateMachine.PhysicsProcess();
    }


}
