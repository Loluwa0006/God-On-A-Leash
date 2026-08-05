using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHitState : PlayerAirState
{
    [SerializeField] int lowHitstunReactionHitstunFrames = 10;
    [SerializeField] int midHitstunReactionHitstunFrames = 25;
    [SerializeField] int highHitstunReactionHitstunFrames = 40;


    public enum HitstunReactionLevel
    {
        None = 0,
        Low = 1,
        Mid = 2,
        High = 3,
    }
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

    public override void AnimationSetup()
    {
        //Determines what animation to player based on hitstun frames.
        HitstunReactionLevel hitstunReactionLevel;
        int hitstunFrames = contactInfo.DamageInfo.hitstunFrames;    
        if (hitstunFrames <= lowHitstunReactionHitstunFrames)
        {
            hitstunReactionLevel = HitstunReactionLevel.Low;
        }
        else if (hitstunFrames <= midHitstunReactionHitstunFrames)
        {
            hitstunReactionLevel = HitstunReactionLevel.Mid;
        }
        else
        {
            hitstunReactionLevel = HitstunReactionLevel.High;
        }
        Player.Animator.SetInteger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Int_HitstunReactionLevel), (int)hitstunReactionLevel);
    }
    void ApplyInvincibility()
    {
        InvulnerabilityEffect invulnerabilityEffect = new(StatusEffectID.PlayerGethitInvulnerability, DamageSource.AnySource, InvulnerabilityEffect.INFINITE_DURATION_VALUE);
        Player.HealthComponent.AddStatusEffect(invulnerabilityEffect);
    }
    void ApplyAttackKnockback()
    {
        var knockbackVector = contactInfo.DamageInfo.GetKnockbackVector(contactInfo.collisionPoint, contactInfo.hurtbox.bounds.center);
        Player.RigidBody.linearVelocity = knockbackVector;
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

    public override void Exit()
    {
        base.Exit();
        Player.Animator.SetInteger(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Int_HitstunReactionLevel), (int)HitstunReactionLevel.None);
    }
}
