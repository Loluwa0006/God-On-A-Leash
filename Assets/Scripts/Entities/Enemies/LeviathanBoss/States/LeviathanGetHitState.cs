using System.Collections.Generic;
using UnityEngine;

public class LeviathanGetHitState : LeviathanBaseState
{

    public enum LeviathanGetHitMessage
    {
        ContactInfo,
    }
    int hitstunTracker = 0;
    HitboxContactInfo contactInfo;
    public override void Enter(Dictionary<string, object> message = null)
    {
        base.Enter(message);
        bool validTransition = false;
        if (message != null)
        {
            if (message.ContainsKey(LeviathanGetHitMessage.ContactInfo.ToString()))
            {
                validTransition = true;
                contactInfo = (HitboxContactInfo)message[LeviathanGetHitMessage.ContactInfo.ToString()];
                hitstunTracker = Mathf.Abs(contactInfo.DamageInfo.hitstunFrames);
            }
        }
        if (!validTransition)
        {
            StateMachine.TransitionTo<PlayerFallState>();
            return;
        }

        ApplyAttackKnockback();
    }

    void ApplyAttackKnockback()
    {
        Vector3 knockbackDirection = (contactInfo.hurtbox.bounds.center - contactInfo.collisionPoint).normalized;
        Vector3 knockbackForce = knockbackDirection * contactInfo.DamageInfo.horizontalKnockback;
        knockbackForce.y = contactInfo.DamageInfo.verticalKnockback;
        Leviathan.RigidBody.linearVelocity = knockbackForce;
    }
    public override void PhysicsProcess()
    {
        hitstunTracker--;
        if (hitstunTracker == 0)
        {
            StateMachine.TransitionTo<LeviathanMoveState>();
            return;
        }
    }
    public override void Process()
    {
        //no auto transitions
    }
    public override bool StateAvailable()
    {
        return false; //special exception, only leviathan controller handles transitions to this state
    }
}
