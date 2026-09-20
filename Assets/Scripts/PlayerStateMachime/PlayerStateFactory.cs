public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext; 
    }

    public PlayerBaseState Combat()
    {
        return new PlayerCombatState(_context, this);
    }
    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Move()
    {
        return new PlayerMoveState(_context, this);
    }
    public PlayerBaseState Speed()
    {
        return new PlayerSpeedState(_context, this);
    }
    public PlayerBaseState Performence()
    {
        return new PlayerPerformenceState(_context, this);
    }
}