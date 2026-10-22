using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class HierarchicalStateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, HierarchicalBaseState<EState>> States = new Dictionary<EState, HierarchicalBaseState<EState>>();
    protected HierarchicalBaseState<EState> _currentState; 
    
    private bool _isTransitioningState;
    
    protected virtual void Start()
    {
        _currentState.EnterState();
    }
    
    protected virtual void Update()
    {
        EState nextStateKey = _currentState.GetNextState();
        if (States[nextStateKey].IsRootState)
        {
            if (nextStateKey.Equals(_currentState.StateKey))
                _currentState.UpdateState();
            
            else
                TransitionToState(nextStateKey);
        }
        if (_currentState.SubState != null)
            _currentState.SubState.UpdateState();
    }
    private void TransitionToState(EState stateKey)
    {
        _currentState.ExitState();
        // new State
        _currentState = States[stateKey];
        _currentState.EnterState();
        
        if (_currentState != null)
            _currentState.SetSubState(States[stateKey]);
    }

    // private void TransitionSubState(EState stateKey)
    // {
    //     _currentSubState = States[stateKey];
    //     _currentRootState.SubState = _currentSubState;
    //     _currentSubState.SuperState = _currentRootState;
    //     _currentSubState.EnterState();
    // }
    
   
}