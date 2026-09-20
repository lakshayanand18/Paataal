using System.Collections.Generic;
using UnityEngine;

public class SeekState : EnemyInteractionState
{
    Vector3 smoothedAvoidance = Vector3.zero;

    public SeekState(EnemyContext context, EnemyStateMachine.EnemyStates estate) : base(context, estate)
    {
        this.Context = context;

    }

    public override void EnterState()
    {
        // clear flags
        Context.IsSeeking = false;
        Context.IsFlocking = false;
        Context.IsAttacking = false;

        // let AttackSlotManager decide if this enemy gets a slot or flocks
        AttackSlotManager.instance.RequestSlot(Context);
    }

    public override void ExitState() { }
    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        // update neighbour list from spatial hash
        SpacialHashMap.instance.GetNeighbours(Context.EnemyTransform.position, myListOfNeighbours);

        // smooth obstacle avoidance so direction changes aren't jarring
        Vector3 rawAvoidance = ObstacleAvoidance(10f, 30f);
        smoothedAvoidance = Vector3.Lerp(smoothedAvoidance, rawAvoidance, Time.deltaTime * .8f);

        // combine steering forces
        Vector3 seek = Seek();
        Vector3 boidDirection = seek + Separation(myListOfNeighbours) + smoothedAvoidance;

        Vector3 velocity = Vector3.zero;
        if (boidDirection != Vector3.zero)
        {
            // rotate towards slot direction
            Context.EnemyTransform.rotation = Quaternion.Slerp(Context.EnemyTransform.rotation, Quaternion.LookRotation(boidDirection),
            Time.deltaTime * Context.TurnSpeed);
            velocity = Vector3.ClampMagnitude(boidDirection * Context.Speed - Context.Rb.linearVelocity, Context.MaxForce);
        }

        Context.Rb.AddForce(velocity);
    }

    public override EnemyStateMachine.EnemyStates GetNextState()
    {
        // sent to flock by AttackSlotManager — no slot available
        if (Context.IsFlocking)
            return EnemyStateMachine.EnemyStates.Flocking;

        // slot reached — transition to attack
        if (Context.IsAttacking)
            return EnemyStateMachine.EnemyStates.Attack;

        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }

    // steer towards assigned slot position
    Vector3 Seek()
    {
        if (!Context.IsSloted)
        {
            return Vector3.zero;
        }

        // recalculate world position every frame so it tracks the player
        Vector3 targetPosition = AttackSlotManager.instance.GetSlotWorldPosition(Context.AssignedSlotIndex, Context.Player);
        Vector3 soloForce = (targetPosition - Context.EnemyTransform.position).normalized;

        // close enough to slot — ready to attack
        if (Vector3.Distance(targetPosition, Context.EnemyTransform.position) <= 20f)
            Context.IsAttacking = true;

        return soloForce * Context.AlignmentWeight;
    }
}

