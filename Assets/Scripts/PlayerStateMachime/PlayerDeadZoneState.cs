using UnityEngine;

public class PlayerDeadZoneState : PlayerCombatBaseState
{
    public PlayerDeadZoneState(PlayerContext context, PlayerCombatStateManager.CombatStates estate) : base(context, estate){}

    public override void EnterState() {}
    public override void ExitState() {}
    public override void UpdateState() {}

    public override void FixedUpdateState()
    {
        _context.Rigidbody.AddForce(0f, 0f, 0f);
        ModelBehavior();
    }
    public override PlayerCombatStateManager.CombatStates GetNextState()
    {
        if (_context.Crosshair.offset.sqrMagnitude >= _context.DeadzoneRadius * _context.DeadzoneRadius)
            return PlayerCombatStateManager.CombatStates.Rotate;
        if(_context.Crosshair.offset.sqrMagnitude < _context.DeadzoneRadius * _context.DeadzoneRadius && _context.Crosshair.lookDelta != Vector2.zero)
            return PlayerCombatStateManager.CombatStates.DeadZoneRotate;
        
        return StateKey;
    }
}
