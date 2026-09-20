using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> CurrentState;
    private bool _isTransitioningState = false;
    
    protected virtual void Start()
    {
        CurrentState.EnterState();
    } 
    protected virtual void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();
        
        if(!_isTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else if (!_isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }
    public void FixedUpdate()
    {
        EState nextStateKey = CurrentState.GetNextState();
        if(!_isTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.FixedUpdateState();
        }
        else if (!_isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }

    public void TransitionToState(EState stateKey)
    {
        _isTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        _isTransitioningState = false;
    }
    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }
    void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }
    void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}