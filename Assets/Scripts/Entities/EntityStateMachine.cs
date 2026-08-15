using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    [SerializeField] Transform owner;
    [SerializeField] BaseState currentState;

    Dictionary<System.Type, BaseState> stateLookup = new();
    List<BaseState> statesWithInactiveProcess = new();
    List<BaseState> statesWithInactivePhysicsProcess = new();

    bool initialized = false;
    public void Initialize()
    {
        foreach (var state in transform.GetComponentsInChildren<BaseState>())
        {
            stateLookup[state.GetType()] = state;
            if (state.hasInactivePhysicsProcess) statesWithInactivePhysicsProcess.Add(state);
            if (state.hasInactiveProcess) statesWithInactiveProcess.Add(state);
            state.InitializeState(this, owner);
        }

        if (currentState == null) currentState = stateLookup.ElementAt(0).Value;
        initialized = true;
    }
    public void Process()
    {
        if (!initialized) return;
        if (currentState != null) currentState.Process();
        foreach (var state in statesWithInactiveProcess)
        {
            if (state == GetCurrentState()) continue;
            state.InactiveProcess();
        }
    }

    public void PhysicsProcess()
    {
        if (!initialized) return;
        if (currentState != null) currentState.PhysicsProcess();
        foreach (var state in statesWithInactivePhysicsProcess)
        {
            if (state == GetCurrentState()) continue;
            state.InactivePhysicsProcess();
        }
    }

    public void TransitionTo<T>(Dictionary<string, object> message = null) where T : BaseState
    {
        if (!initialized) return;
        if (!stateLookup.ContainsKey(typeof(T)))
        {
            Debug.LogWarning("Could not find object of type " + typeof(T));
            return;
        }
        var newState = stateLookup[typeof(T)];
        if (newState == currentState)
        {
            return;
        }
        var previousState = currentState;   
        currentState.Exit();
        currentState = newState;
        currentState.Enter(message);

        previousState.AnimationTeardown();
        currentState.AnimationSetup();
    }

    public void TransitionTo(System.Type state, Dictionary<string, object> message = null)
    {
        if (!initialized) return;
        if (!stateLookup.ContainsKey(state))
        {
            Debug.LogWarning("Could not find object of type " + state);
            return;
        }
        var newState = stateLookup[state];
        if (newState == currentState)
        {
            return;
        }
        var previousState = currentState;
        currentState.Exit();
        currentState = newState;
        currentState.Enter(message);

        previousState.AnimationTeardown();
        currentState.AnimationSetup();
    }
    public BaseState GetCurrentState()
    {
        return currentState;
    }

    public bool IsStateAvailable<T>() where T : BaseState
    {
        if (!initialized) return false;
        if (!stateLookup.ContainsKey(typeof(T)))
        {
            return false;
        }
        return stateLookup[typeof(T)].StateAvailable();
    }

    public bool IsStateAvailable(System.Type type)
    {
        if (!initialized) return false;
        if (!stateLookup.ContainsKey(type))
        {
            return false;
        }
        return stateLookup[type].StateAvailable();
    }
}
