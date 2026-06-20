
public class LeviathanEntity : BaseEnemy
{

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
