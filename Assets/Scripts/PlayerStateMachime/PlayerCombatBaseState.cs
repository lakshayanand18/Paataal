using UnityEngine;
using System.Collections;


public abstract class PlayerCombatBaseState : BaseState<PlayerCombatStateManager.CombatStates>
{
    protected PlayerContext  _context;

    public PlayerCombatBaseState(PlayerContext context, PlayerCombatStateManager.CombatStates stateKey) : base(stateKey)
    {
       _context = context;
    }
    protected void ModelBehavior() 
    {
        Vector3 offset2D = new Vector3(-_context.Crosshair.offset.x /530f * 10f,-_context.Crosshair.offset.y / 530f * 8f, 0f);
        _context.PlayerModel.localPosition = Vector3.Lerp(_context.PlayerModel.localPosition, offset2D, Time.fixedDeltaTime * _context.ModelOffsetSpeed);
        //rotate player model to face movement direction
        Quaternion targetRotation = Quaternion.Euler(-_context.Crosshair.offset.y / 530f * 45f, _context.Crosshair.offset.x / 530f * 45f, -_context.Crosshair.offset.x / 530f * 30f);
        _context.PlayerModel.localRotation = Quaternion.Slerp(_context.PlayerModel.localRotation, targetRotation, Time.fixedDeltaTime * _context.PlayerModelRotationSpeed);
    }
}
