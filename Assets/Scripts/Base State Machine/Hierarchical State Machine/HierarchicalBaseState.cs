using System;

public abstract class HierarchicalBaseState<EState> where EState : Enum 
{
    public HierarchicalBaseState<EState> SuperState { get; set; }
    public HierarchicalBaseState<EState> SubState { get; set; }
    public bool IsRootState { get; private set; }
    public EState StateKey { get; private set; }

    public HierarchicalBaseState(EState state, bool isRootState)
    {
        StateKey = state;
        IsRootState = isRootState;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract EState GetNextState();

    public void UpdateStates()
    {
        UpdateState();

        if (SubState != null)
        {
            SubState.UpdateStates();
        }
    }

    // protected void SwitchState(HierarchicalBaseState<EState> newState)
    // {
    //     //current state exits state
    //     ExitState();
    //     //new State enter
    //     newState.EnterState();
    //     
    //     if (IsRootState)
    //     {
    //         //switch current state of context
    //         _ctx.CurrentState = newState;
    //     }
    //     else if (_currentSuperState != null)
    //     {
    //         _currentSuperState.SetSubState(newState);
    //     }
    //
    // }
    
    public void SetSubState(HierarchicalBaseState<EState> newSubState)
    {
        SubState = newSubState;
        newSubState.SetSuperState(this);
        //makes sure the new substate actually enters
        newSubState.EnterState();
    }

    public void SetSuperState(HierarchicalBaseState<EState> newSuperState)
    {
        SuperState = newSuperState;
    }
   
}