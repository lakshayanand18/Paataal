using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        Ctx.MovementSpeed = 17500f;
        Ctx.Rb.linearDamping = 5f;
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public override void ExitState()
    { 
        
    }
    public override void InitializeSubState() { }
    public override void CheckSwitchState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsSpeedPressed && !(Currentsubstate is PlayerIdleState))
        {
            SwitchState(Factory.Idle());
        }
        if (Ctx.IsMovementPressed && Ctx.IsSpeedPressed && !(Currentsubstate is PlayerSpeedState))
        {
            SwitchState(Factory.Speed());
        }
    }
}
