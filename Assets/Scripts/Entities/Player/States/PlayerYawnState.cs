using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerYawnState : PlayerAirState
{

    float timeRemainingAfterAnarchyGenerated = 0;

    int elaspedYawnTime;
    int minYawnTime;

    [SerializeField] UnityEvent justYawnPerformed = new();

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerShadowstepState),
        };
    }

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        Player.AnarchyManager.anarchyGainedThroughScaledMethod.AddListener((method, charges) => OnAnarchyGenerated());
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        //no holding, cheater!
        if (timeRemainingAfterAnarchyGenerated > 0 && !Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].ActionPressed)
        {
            OnJustYawn();
        }
        else
        {
            minYawnTime = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinYawnTime);
        }
  
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn].Consume();
        elaspedYawnTime = 0;
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsYawning), true);
    }
    void OnJustYawn()
    {
        minYawnTime = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinJustYawnTime);
        Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.JustYawn);
        Player.RodManager.RodLength = 0.0f;
        justYawnPerformed.Invoke();
    }

    public override void PhysicsProcess()
    {
        elaspedYawnTime++;
        if (elaspedYawnTime >= minYawnTime && !Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn].ActionPressed)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.AnarchyManager.GenerateAnarchyUnscaled(UnscaledGenerationMethod.Yawn);
        float gravity;
        if (Player.RigidBody.linearVelocity.y > 0) gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID);
        else gravity = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.FALL_GRAVITY_ID);
        ApplyGravity(gravity);
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerYawnAirAcceleration));
        Player.RodManager.RodLength = Mathf.MoveTowards(Player.RodManager.RodLength, 0.0f, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerRodRetractionSpeedWhileYawning));
    }

    public override void InactivePhysicsProcess()
    {
        //timeRemainingAfterAnarchyGenerated = Mathf.MoveTowards(timeRemainingAfterAnarchyGenerated, 0, 1);
        timeRemainingAfterAnarchyGenerated--;
    }


    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_IsYawning), false);
    }
    public override bool StateAvailable()
    {
        var yawnBuffer = Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Yawn];
        return !Player.PlayerGrounded
            && yawnBuffer.Buffered || yawnBuffer.ActionPressed;
    }
    void OnAnarchyGenerated()
    {
        timeRemainingAfterAnarchyGenerated = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerJustYawnWindow);
        Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerJustYawnWindow);

    }

}
