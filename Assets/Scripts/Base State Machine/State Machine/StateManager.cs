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
    protected virtual void FixedUpdate()
    {
        CurrentState.FixedUpdateState();
    }

    public void TransitionToState(EState stateKey)
    {
        _isTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        _isTransitioningState = false;
    }
}