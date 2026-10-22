using UnityEngine;

public class PlayerMoveState : PlayerMovementBaseState
{
    PlayerContext _context;
    public PlayerMoveState(PlayerContext context, PlayerMovementStateManager.MovementStates estate) : base (context, estate)
    {
        _context = context;
    }
    public override void EnterState()
    {
        _context.MovementSpeed = 17500f;
        _context.Rigidbody.linearDamping = 5f;
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    { 
        
    }

    public override void FixedUpdateState()
    {
        ProcessMovement();
    }

    public override PlayerMovementStateManager.MovementStates GetNextState()
    {
        if (!_context.IsMovementPressed && !_context.IsSpeedPressed)
            return PlayerMovementStateManager.MovementStates.Idle;
        if(_context.IsMovementPressed && _context.IsSpeedPressed)
            return PlayerMovementStateManager.MovementStates.Speed;
        return StateKey;
    }
}
