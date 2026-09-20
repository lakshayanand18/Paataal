using UnityEngine;

public class PlayerSpeedState : PlayerBaseState
{
    public PlayerSpeedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory){}
    public override void EnterState()
    {
        Ctx.MovementSpeedModifier = 1.5f;
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public override void ExitState()
    {
        Ctx.MovementSpeedModifier = 1f;
    }
    public override void InitializeSubState(){ }
    public override void CheckSwitchState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerIdleState))
        {
            SwitchState(Factory.Idle());
        }
        if (Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerMoveState))
        {
            SwitchState(Factory.Move());
        }
    }
}
