using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowstepState : PlayerBaseState
{
    float initialSpeed;

    int durationTracker = 0;

    bool startedAtMaxCharge = false;
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        startedAtMaxCharge = Player.SquashbucklerManager.SquashbucklerCharge == Player.SquashbucklerManager.MaxCharge;
        initialSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z).magnitude;
        if (initialSpeed < Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinimumShadowstepSpeed)) initialSpeed = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinimumShadowstepSpeed);
        durationTracker = (int)(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerDurationPerSquashbucklerCharge) * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode));
        Player.SquashbucklerManager.SquashbucklerCharge -= (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Squashbuckler].Consume();
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume(); //prevent accidental dragonslashes
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedShadowstep));
    }
    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        Player.RigidBody.linearVelocity = initialSpeed * viewCamera.transform.forward;
        durationTracker--;
        if (durationTracker == 0)
        {
            Player.SquashbucklerManager.SquashbucklerCharge--;
            durationTracker = (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerDurationPerSquashbucklerCharge);
            if (!Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Squashbuckler].ActionPressed)
            {
                FallFromShadowstep();
                return;
            }
        }
        if (Player.SquashbucklerManager.SquashbucklerCharge == 0)
        {
            FallFromShadowstep();
            return;
        }
        if (startedAtMaxCharge && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Buffered)
        {
            StateMachine.TransitionTo<PlayerDragonslashState>();
            return;
        }
    }

    void FallFromShadowstep()
    {
        StateMachine.TransitionTo<PlayerFallState>();
    }

    public override void Process()
    {
  
    }

    public override void Exit()
    {
        base.Exit();
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Shadowstep);
    }

    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedShadowstep));
    }
    public override bool StateAvailable()
    {
        return Player.SquashbucklerManager.SquashbucklerCharge > (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode) 
               && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Squashbuckler].Buffered;
    }
}
