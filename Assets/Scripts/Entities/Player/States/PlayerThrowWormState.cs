using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerThrowWormState : PlayerAirState
{ 
    [SerializeField] LayerMask terrainMask;
    
    int durationTracker = 0;

    public override Type[] statesToAttemptToTransitionTo
    {
        get => new Type[]
        {
            typeof(PlayerSwingState),
            typeof (PlayerDashState),

        };
        protected set => base.statesToAttemptToTransitionTo = value;
    }
 
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        Vector3 newSpeed = Player.RigidBody.linearVelocity;
        float wormJumpPower = Player.StatsManager.GetValueFromStat(StatID.WormJumpPower);
        newSpeed.y = Mathf.Max(newSpeed.y + wormJumpPower, wormJumpPower);
        Player.RigidBody.linearVelocity = newSpeed;
        durationTracker = (int) Player.StatsManager.GetValueFromStat(StatID.WormThrowDuration);
        if (Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.FireWormRail].Buffered && Player.WormManager.WormsRemaining >= Player.StatsManager.GetValueFromStat(StatID.WormsRequiredForRail))
        {
            FireWorm(Player.WormManager.GetNewWormRail(), cost: (int)Player.StatsManager.GetValueFromStat(StatID.WormsRequiredForRail));
        }
        else
        {
            FireWorm(Player.WormManager.GetNewWorm(), cost: 1);
        }
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.FireWorm].Consume();
        Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.FireWormRail].Consume();
    }

    void FireWorm(WormEntity worm, int cost)
    {
        var cameraRay = viewCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        float wormThrowRange = Player.StatsManager.GetValueFromStat(StatID.WormThrowRange);
        var raycast = Physics.Raycast(cameraRay, out var hitInfo, wormThrowRange, terrainMask, QueryTriggerInteraction.Collide);
        Vector3 wormTarget;
        if (raycast)
        {
            wormTarget = hitInfo.point;
        }
        else
        {
            wormTarget = cameraRay.GetPoint(wormThrowRange);
        }
        worm.Fire(wormTarget, Player.transform.position, Player.RigidBody.linearVelocity);
        Player.WormManager.WormsRemaining -= cost;
    }
    public override void PhysicsProcess()
    {

        base.PhysicsProcess();
        if (Player.RigidBody.linearVelocity.y > 0)
        {
            ApplyGravity(Player.StatsManager.GetValueFromStat(StatID.WormJumpGravity));
        }
        else
        {
            ApplyGravity(Player.StatsManager.GetValueFromStat(StatID.WormFallGravity));
        }
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatID.PlayerAirAcceleration));
        durationTracker--;
        if (durationTracker == 0)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        Player.PlayerGrounded = IsGrounded();
        if (Player.PlayerGrounded)
        {
            if (StateMachine.IsStateAvailable<PlayerRunState>())
            {
                StateMachine.TransitionTo<PlayerRunState>();
            }
            else
            {
                StateMachine.TransitionTo<PlayerIdleState>();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Player.AnarchyManager.GenerateAnarchy(ScaledGenerationMethod.WormThrow);
    }

    public override bool StateAvailable()
    {

        if (Player.WormManager.WormsRemaining > 0 && Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.FireWorm].Buffered ||  Player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.FireWormRail].Buffered)
        {
            return true;
        }
            return false;
    }
}
