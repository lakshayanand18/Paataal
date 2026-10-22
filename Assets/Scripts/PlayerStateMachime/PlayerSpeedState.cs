using UnityEngine;

public class PlayerSpeedState : PlayerMovementBaseState
{
    private PlayerContext _context;

    public PlayerSpeedState(PlayerContext context, PlayerMovementStateManager.MovementStates estate) : base(context, estate)
    {
        _context = context;
    }
    public override void EnterState()
    {
        _context.MovementSpeedModifier = 1.5f;
        _context.MovementSpeed *= _context.MovementSpeedModifier;
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        _context.MovementSpeedModifier = 1f;
    }

    public override void FixedUpdateState()
    {
        ProcessMovement();
    }

    public override PlayerMovementStateManager.MovementStates GetNextState()
    {
        if (!_context.IsMovementPressed && !_context.IsSpeedPressed)
        {
            return PlayerMovementStateManager.MovementStates.Idle;
        }

        if (_context.IsMovementPressed && !_context.IsSpeedPressed)
        {
            return PlayerMovementStateManager.MovementStates.Move;
        }
        return StateKey;
    }
}
