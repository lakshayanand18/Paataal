using System.Collections;
using UnityEngine;

public class AttackState : EnemyInteractionState
{
    public AttackState(EnemyContext context, EnemyStateMachine.EnemyStates estate) : base(context, estate)
    {
        EnemyContext Context = context;
    }
    public override void EnterState()
    {
        Context.IsLeader = false;

        if (Context.IsSloted && !Context.IsAttackingCoroutineRunning)
        {
            Context.MonoBehaviour.StartCoroutine(Attack(Context));
        }
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        Context.EnemyTransform.LookAt(Context.Player);
    }
    public override void FixedUpdateState()
    {
        
    }
    public override EnemyStateMachine.EnemyStates GetNextState()
    {
        if (Context.IsSloted)
        {
            Vector3 slotWorldPos = AttackSlotManager.instance.GetSlotWorldPosition(Context.AssignedSlotIndex, Context.Player);

            // Player moved far enough that we need to re-approach
            if (Vector3.Distance(Context.EnemyTransform.position, slotWorldPos) > 20f)
            {
                AttackSlotManager.instance.ReleaseSlot(Context);
                Context.IsAttacking = false;
                return EnemyStateMachine.EnemyStates.Seek;
            }
        }

        return StateKey;
    }
    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }

    IEnumerator Attack(EnemyContext other)
    {
        while(true)
        {   other.IsAttackingCoroutineRunning = true;
            other.IsAttackingPlayer = true;
            Context.PlayerStateMachine.PlayerDamage(5);

            yield return new WaitForSeconds(1f);

            other.IsAttackingPlayer = false;;
            yield return new WaitForSeconds(5f);
            
            other.IsAttackingCoroutineRunning = false;
        }
    }

}
