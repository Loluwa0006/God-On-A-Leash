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


    private void Start()
    {
        foreach (var state in transform.GetComponentsInChildren<BaseState>())
        {
            stateLookup[state.GetType()] = state;
            if (state.hasInactivePhysicsProcess) statesWithInactivePhysicsProcess.Add(state);
            if (state.hasInactiveProcess) statesWithInactiveProcess.Add(state);
            state.InitializeState(this, owner);
        }

        if (currentState == null) currentState = stateLookup.ElementAt(0).Value;
    }

    public void Process()
    {
        if (currentState != null) currentState.Process();
        foreach (var state in statesWithInactiveProcess)
        {
            if (state == GetCurrentState()) continue;
            state.InactiveProcess();
        }
    }

    public void PhysicsProcess()
    {
        if (currentState != null) currentState.PhysicsProcess();
        foreach (var state in statesWithInactivePhysicsProcess)
        {
            if (state == GetCurrentState()) continue;
            state.InactivePhysicsProcess();
        }
    }

    public void TransitionTo<T>(Dictionary<string, object> message = null) where T : BaseState
    {
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

        currentState.Exit();
        currentState = newState;
        currentState.Enter(message);
    }

    public void TransitionTo(System.Type state, Dictionary<string, object> message = null)
    {
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
        currentState.Exit();
        currentState = newState;
        currentState.Enter(message);
    }
    public BaseState GetCurrentState()
    {
        return currentState;
    }

    public bool IsStateAvailable<T>() where T : BaseState
    {
        if (!stateLookup.ContainsKey(typeof(T)))
        {
            return false;
        }
        return stateLookup[typeof(T)].StateAvailable();
    }

    public bool IsStateAvailable(System.Type type)
    {
        if (!stateLookup.ContainsKey(type))
        {
            return false;
        }
        return stateLookup[type].StateAvailable();
    }
}
