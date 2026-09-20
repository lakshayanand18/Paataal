public class PlayerCombatState : PlayerBaseState
{
    public PlayerCombatState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public override void ExitState()
    {

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
        if (Ctx.IsPerformenceAndCombatPressed)
        {
            SwitchState(Factory.Performence());
        }
    }
}