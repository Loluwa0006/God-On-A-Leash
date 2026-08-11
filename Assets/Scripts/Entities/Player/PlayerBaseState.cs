using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : BaseState
{
    public const float MOVEMENT_DEADZONE = 0.1f;

    public const float GROUND_CHECK_SAFE_MARGIN = 0.15f;

    const float SHAPECAST_RATIO = 0.8f;

    protected static LayerMask groundMask;

    public PlayerController Player { private set; get; }


    public virtual Type[] statesToAttemptToTransitionTo { get; protected set; }


    protected static Camera viewCamera;


  
    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        Player = owner.GetComponent<PlayerController>();
        groundMask = LayerMask.GetMask("Ground", "Swingable");
        if (viewCamera == null ) viewCamera = Camera.main;
    }
    public bool IsGrounded()
    {
        if (Player == null) return false;
        var ray = new Ray(Player.Collider.bounds.center, Vector3.down);
        bool hit = Physics.SphereCast
            (
            ray, 
            Player.Collider.bounds.extents.x * SHAPECAST_RATIO,
            Player.Collider.bounds.extents.y + GROUND_CHECK_SAFE_MARGIN,
            groundMask
            );
        return hit;
    }

    public override void Process()
    {
        base.Process();
        AttemptStateTransition();
    }

    protected void AttemptStateTransition()
    {
        for (int i = 0; i < statesToAttemptToTransitionTo.Length; i++)
        {
            var stateClass = statesToAttemptToTransitionTo[i];
            if (StateMachine.IsStateAvailable(stateClass))
            {
                StateMachine.TransitionTo(stateClass, null);
                return;
            }
        }
    }

    public virtual void OnPlayerStruck(HitboxContactInfo info)
    {

        Dictionary<string, object> getHitStateMessage = new()
        {
            [PlayerGetHitState.PlayerGetHitMessage.ContactInfo.ToString()] = info
        };
        StateMachine.TransitionTo<PlayerGetHitState>(getHitStateMessage);
    }

}
