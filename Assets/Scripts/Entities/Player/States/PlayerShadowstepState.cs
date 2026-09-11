using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PlayerShadowstepState : PlayerBaseState
{
    [SerializeField] List<SkinnedMeshRenderer> playerMeshes = new();
    [SerializeField] Material shadowMaterial;

    [SerializeField] VisualEffect shadowTrail;
    float shadowstepSpeed;

    int durationTracker = 0;

    bool startedAtMaxCharge = false;

    Material previousMaterial = null;

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        shadowTrail.Stop();
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        startedAtMaxCharge = Player.SquashbucklerManager.SquashbucklerCharge == Player.SquashbucklerManager.MaxCharge;
        shadowstepSpeed = Player.RigidBody.linearVelocity.magnitude;
        if (shadowstepSpeed < Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinimumShadowstepSpeed)) shadowstepSpeed = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinimumShadowstepSpeed);
        durationTracker = (int)(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerDurationPerSquashbucklerCharge) * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode));
        Player.SquashbucklerManager.SquashbucklerCharge -= (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode);
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Squashbuckler].Consume();
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Slash].Consume(); //prevent accidental dragonslashes
        previousMaterial = playerMeshes[0].material;
        foreach (var mesh in playerMeshes) mesh.material = shadowMaterial;
        shadowTrail.Play();
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsFalling));
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedShadowstep));
    }
    public override void PhysicsProcess()
    {
        base.PhysicsProcess();
        Player.RigidBody.linearVelocity = shadowstepSpeed * viewCamera.transform.forward;
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
        foreach (var mesh in playerMeshes) mesh.material = previousMaterial;
        shadowTrail.Stop();
    }

    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedShadowstep));
    }
    public override bool StateAvailable()
    {
        return Player.SquashbucklerManager.SquashbucklerCharge >= (int) Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode) 
               && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Squashbuckler].Buffered;
    }
}
