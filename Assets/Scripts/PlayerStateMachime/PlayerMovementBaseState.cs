using UnityEngine;

public abstract class PlayerMovementBaseState : BaseState<PlayerMovementStateManager.MovementStates>
{
    private PlayerContext  _context;

    public PlayerMovementBaseState(PlayerContext context, PlayerMovementStateManager.MovementStates stateKey) : base(stateKey)
    {
       _context = context;
    }
    
    protected void ProcessMovement() 
    {
        _context.Rigidbody.AddRelativeForce(_context.MovementInput * (_context.MovementSpeed * Time.fixedDeltaTime));
    }
}
