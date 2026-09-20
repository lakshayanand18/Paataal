public class PlayerPerformenceState : PlayerBaseState
{
    public PlayerPerformenceState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.MovementSpeedModifier = 2f;
    }
    public override void UpdateState()
    { 
        CheckSwitchState();
    }
    public override void ExitState()
    {
        Ctx.MovementSpeedModifier = 1f;
    }
    public override void InitializeSubState()
    { 
        if (Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerMoveState))
        {
            SetSubState(Factory.Move());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerIdleState))
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsSpeedPressed && !(Currentsubstate is PlayerSpeedState))
        {
            SetSubState(Factory.Speed());
        }
    }
    public override void CheckSwitchState()
    {
         if (!Ctx.IsPerformenceAndCombatPressed)
        {
            SwitchState(Factory.Combat());
        }
    }
}