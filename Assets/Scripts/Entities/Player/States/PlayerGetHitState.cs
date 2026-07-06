using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHitState : PlayerAirState
{
   
    public enum PlayerGetHitMessage
    {
        ContactInfo,
    }
    int hitstunTracker = 0;
    int invulnerablityTracker = 0;
    HitboxContactInfo contactInfo;
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        bool validTransition = false;
        if (message != null)
        {
            if (message.ContainsKey(PlayerGetHitMessage.ContactInfo.ToString()))
            {
                validTransition = true;
                contactInfo = (HitboxContactInfo)message[PlayerGetHitMessage.ContactInfo.ToString()];
                hitstunTracker = Mathf.Abs(contactInfo.DamageInfo.hitstunFrames);
            }
        }
        if (!validTransition)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }

        ApplyAttackKnockback();
        ApplyInvincibility();
        invulnerablityTracker = (int)Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.ExtraInvulnerabilityFramesAfterHit);
    }
    void ApplyInvincibility()
    {
        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.PlayerGethitInvulnerability, DamageSource.AnySource, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect);
    }
    void ApplyAttackKnockback()
    {
       Player.RigidBody.linearVelocity = contactInfo.DamageInfo.GetKnockbackVector(contactInfo.collisionPoint, contactInfo.hurtbox.bounds.center);
    }
    public override void PhysicsProcess()
    {
        hitstunTracker--;
        if (hitstunTracker == 0 )
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }
        //Use jump gravity because it's more forgiving: the force is weaker and gives the player
        //more opportunity to recover.
        ApplyGravity(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGroundedJumpInfo, JumpInfo.JUMP_GRAVITY_ID));
        AirborneMovement(Player.PlayerInput.GetMovementDirection(), Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAirAcceleration));
    }

    public override void InactivePhysicsProcess()
    {
        invulnerablityTracker--;
        if (invulnerablityTracker == 0)
        {
            Player.HealthComponent.RemoveStatusEffect(StatusEffectID.PlayerGethitInvulnerability);
        }
    }

    public override void Process()
    {
        //no auto transitions
    }
    public override bool StateAvailable()
    {
        return false; //special exception, only player controller handles transitions to this state
    }
}
