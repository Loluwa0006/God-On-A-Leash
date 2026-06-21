
public class LeviathanEntity : BaseEnemy
{

    public enum AnimationParameter
    {
        Trigger_IsAttacking,
        Bool_InDreamphase
    }
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

    public string GetAnimationParameterFormatted(AnimationParameter parameter)
    {
        var parameterString = parameter.ToString();
        parameterString = parameterString.Substring(parameterString.IndexOf("_") + 1);
        return parameterString;
    }


}
