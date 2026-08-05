using System.Collections.Generic;
using UnityEngine;

public class BaseState : MonoBehaviour
{
    public bool hasInactiveProcess = false;
    public bool hasInactivePhysicsProcess = false;

    public EntityStateMachine StateMachine { set; get; }

    public virtual void InitializeState(EntityStateMachine stateMachine, Transform owner)
    {
        StateMachine = stateMachine;
    }

    public virtual void AnimationSetup()
    {

    }
    public virtual void Enter(Dictionary<string, object> message = null)
    {

    }

    public virtual void Process()
    {

    }

    public virtual void PhysicsProcess()
    {

    }

    public virtual void InactiveProcess()
    {

    }

    public virtual void InactivePhysicsProcess()
    {

    }

    public virtual bool StateAvailable()
    {
        return false;
    }

    public virtual void Exit()
    {

    }
    public virtual void AnimationTeardown()
    {

    }
}
