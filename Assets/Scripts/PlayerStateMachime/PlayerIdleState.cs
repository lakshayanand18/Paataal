using UnityEngine;

public class PlayerIdleState : PlayerMovementBaseState
{
    private PlayerContext _context;
    public PlayerIdleState(PlayerContext context, PlayerMovementStateManager.MovementStates estate) : base(context ,estate)
    {
        _context = context;
    }
    public override void EnterState()
    {
        _context.Rigidbody.linearDamping = 20f;
        _context.MovementSpeed = 0f;
    }
    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        ProcessMovement();
    }

    public override void ExitState()
    {
        _context.Rigidbody.linearDamping = 0f;
    }

    public override PlayerMovementStateManager.MovementStates GetNextState()
    {
        if(_context.IsMovementPressed && !_context.IsSpeedPressed)
            return PlayerMovementStateManager.MovementStates.Move;
        if(_context.IsMovementPressed && _context.IsSpeedPressed)
            return PlayerMovementStateManager.MovementStates.Speed;
        return StateKey;
    }
}
