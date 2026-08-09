using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDashState : PlayerAirState
{
    [SerializeField] float cameraTransitionTime = 0.1f;
    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerSlashState),
            typeof(PlayerThrowWormState),
            typeof(PlayerShadowstepState),
        };
    }

    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Player.RodManager.StartDash();
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Dash].Consume();
        Player.CameraManager.TransitionToCamera(Player.CameraManager.CloseFollowCamera, cameraTransitionTime);
        Vector3 lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z);
    }

    public override void AnimationSetup()
    {
        base.AnimationSetup();
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_JumpPerformed));
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedDashing));

    }

    public override void PhysicsProcess()
    {

        var dashDirection = Player.PlayerInput.GetMovementDirection().y;

        var dashDirectionCorrected = Mathf.Clamp(dashDirection, 0, 1); //makes neutral and holding back the same value


        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerDashLateralAcceleration));

        var directionToGrapple = (Player.RodManager.GrappleInfo.GrapplePosition - Player.Collider.bounds.center).normalized;

        float minDashPower = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinimumDashPower);
        float maxDashPower = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaximumDashPower);
        float dashPower = Mathf.Lerp(minDashPower, maxDashPower, dashDirectionCorrected);

        Player.RigidBody.AddForce(directionToGrapple * dashPower, ForceMode.VelocityChange);

        float distanceFromGrapplePoint = Vector3.Distance(Player.RodManager.GrappleInfo.GrapplePosition, Player.Collider.bounds.center);
        float distanceBeforeCancel = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinDistanceBeforeDashCancelled);

        if (distanceFromGrapplePoint <= distanceBeforeCancel)
        {
            Player.RodManager.RodLength = 0.0f;
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        if (!Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Dash].ActionPressed)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }

        Player.RodManager.RodLength = distanceFromGrapplePoint;
    }

 

    public override void Exit()
    {
        base.Exit();
        Player.RodManager.DisableGrapple();
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.Dash);
        Player.CameraManager.TransitionToCamera(Player.CameraManager.DefaultCamera, cameraTransitionTime);
    }
    public override void AnimationTeardown()
    {
        base.AnimationTeardown();
        Player.Animator.SetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_JumpPerformed)); 
        Player.Animator.ResetTrigger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Trigger_StartedDashing));
    }
    public override bool StateAvailable()
    {
        if (GrappleUtilities.AimingAtGrappable(Player, Player.RodManager.GrappleMask) && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.Dash].Buffered)
        {
            if (Vector3.Distance(GrappleUtilities.RaycastResult.point, Player.Collider.bounds.center) >= (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinDistanceBeforeDashCancelled))
            {
                return true;
            }
        }
        return false;
    }
}
