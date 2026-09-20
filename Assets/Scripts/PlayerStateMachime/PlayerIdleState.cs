public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){}
    public override void EnterState()
    {
        Ctx.Rb.linearDamping = 20f;
        Ctx.MovementSpeed = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public override void ExitState()
    {
        Ctx.Rb.linearDamping = 0f;
    }
    public override void InitializeSubState(){}
    public override void CheckSwitchState()
    {
        if (Ctx.IsMovementPressed && Ctx.IsSpeedPressed && !(Currentsubstate is PlayerSpeedState))
        {
            SwitchState(Factory.Speed());
        }
        if (Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerMoveState))
        {
            SwitchState(Factory.Move());
        }
    }
    

}
