using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using System;
public class PlayerRailParryState : PlayerBaseState
{
    [SerializeField] LayerMask railLayer;
    [SerializeField] SphereCollider railCollider;
    [SerializeField] SplineAnimate splineAnimator;
    [SerializeField] float cameraTransitionTime = 0.35f;
    [SerializeField] Vector3 railPositionOffset = new Vector3(0, 1.5f, 0);
    SplineContainer splineToFollow;

    float splineDirection;


    float splineLength;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
           typeof(PlayerShadowstepState), 
        };
    }

    Collider[] railCheck = new Collider[1];

    RigidbodyInterpolation previousInterpolation;

    public override void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        base.InitializeState(stateMachine, owner);
        if (splineAnimator == null) splineAnimator = Player.GetComponent<SplineAnimate>();
        splineAnimator.AnimationMethod = SplineAnimate.Method.Speed;
        splineAnimator.enabled = false;
    }
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);

        previousInterpolation = Player.RigidBody.interpolation;
        Player.RigidBody.interpolation = RigidbodyInterpolation.None;
        if (splineToFollow == null)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        splineAnimator.enabled = true;

        InitializeSplineMovement();
        Player.PlayerGrounded = true;
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].Consume();
        Player.RigidBody.isKinematic = false;

        Player.Model.transform.localPosition = railPositionOffset;
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_GrindingRail), true);
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_JumpPerformed));
    }

    void InitializeSplineMovement()
    {
        var pointInLocalSpace = splineToFollow.transform.InverseTransformPoint(Player.Collider.bounds.center);
        SplineUtility.GetNearestPoint(splineToFollow.Spline, pointInLocalSpace, out float3 startPosition, out float time);
        Vector3 tangent = Vector3.Normalize(SplineUtility.EvaluateTangent(splineToFollow.Spline, time));
        var velocityProjectedOntoSpline = Vector3.Dot(tangent, Player.RigidBody.linearVelocity.normalized);
        splineDirection = Mathf.Sign(velocityProjectedOntoSpline);
        splineAnimator.Container = splineToFollow;
        splineAnimator.MaxSpeed = Mathf.Abs(
            Mathf.Max(Player.RigidBody.linearVelocity.magnitude * Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PreviousSpeedToRailSpeedRatio) * velocityProjectedOntoSpline,
            Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.RailParryMinimumSpeed))) ;
        splineAnimator.NormalizedTime = time;
        splineLength = splineToFollow.CalculateLength();
    }

    public override void PhysicsProcess()
    {
       float delta = splineAnimator.MaxSpeed / splineLength;
       float timeToAdd = (delta * splineDirection) * Time.fixedDeltaTime;
        splineAnimator.NormalizedTime = Mathf.Clamp01(splineAnimator.NormalizedTime + timeToAdd);
        if (splineAnimator.NormalizedTime > 0.99f || splineAnimator.NormalizedTime < 0.01f || !Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].ActionPressed)
        {
            Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_RailJumpPerformed));
            StateMachine.TransitionTo<PlayerFallState>();
        }
    }


    Vector3 CalculateExitVelocity(Vector3 tangent)
    {
        Vector3 normalizedTangent = Vector3.Normalize(tangent);

        Vector3 exitVelocity = splineAnimator.MaxSpeed * splineDirection * normalizedTangent;
        float railParryMinimumJump = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.RailParryMinimumJump);
        if (exitVelocity.y < railParryMinimumJump) exitVelocity.y = railParryMinimumJump;

        return exitVelocity;
    }

    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_GrindingRail), false);
    }
    public override void Exit()
    {
        base.Exit();
        splineAnimator.enabled = false;
        Player.RigidBody.isKinematic = false;
        SplineUtility.Evaluate(splineToFollow.Spline, splineAnimator.NormalizedTime, out float3 position, out float3 tangent, out float3 upVector);
        Player.RigidBody.linearVelocity = CalculateExitVelocity(tangent);
        Player.RigidBody.rotation = Quaternion.LookRotation(Player.RigidBody.linearVelocity.normalized);
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.RailParry);
       // Player.CameraManager.TransitionToCamera(Player.CameraManager.DefaultCamera, cameraTransitionTime);
        railCheck[0] = null;
        Player.Model.transform.localPosition = Vector3.zero;
        Player.RigidBody.interpolation = previousInterpolation;

    }
    public override bool StateAvailable()
    {
        if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Parry].ActionPressed)
        {
            int overlap = Physics.OverlapSphereNonAlloc(railCollider.bounds.center, railCollider.radius, railCheck, railLayer, QueryTriggerInteraction.Collide);
            if (overlap > 0 && railCheck[0] != null)
            {
                splineToFollow = railCheck[0].GetComponent<SplineContainer>();
                if (splineToFollow == null) splineToFollow = railCheck[0].transform.parent.GetComponent<SplineContainer>();
                return true;
            }
        }
        return false;
    }
}
