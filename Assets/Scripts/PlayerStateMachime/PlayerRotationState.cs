using UnityEngine;

public class PlayerRotationState : PlayerCombatBaseState
{
    private CrossMovement _crosshair;

    public PlayerRotationState(PlayerContext context, PlayerCombatStateManager.CombatStates estate) : base(context, estate) {}

    public override void EnterState() {}
    public override void ExitState() {}
    public override void UpdateState()
    {
       
    }

    public override void FixedUpdateState()
    {
        Vector3 torque = new Vector3(-_context.Crosshair.offset.y, _context.Crosshair.offset.x, 0f) * (_context.RotationSpeed *  Time.fixedDeltaTime);   
        _context.Rigidbody.AddRelativeTorque(torque, ForceMode.Force);
        ModelBehavior();
    }
    public override PlayerCombatStateManager.CombatStates GetNextState()
    {
        if (_context.Crosshair.offset.sqrMagnitude < _context.DeadzoneRadius * _context.DeadzoneRadius && _context.Crosshair.lookDelta == Vector2.zero)
            return PlayerCombatStateManager.CombatStates.DeadZone;
        if(_context.Crosshair.offset.sqrMagnitude < _context.DeadzoneRadius * _context.DeadzoneRadius && _context.Crosshair.lookDelta != Vector2.zero)
            return PlayerCombatStateManager.CombatStates.DeadZoneRotate;
        
        return StateKey;
    }
}