using System.Collections.Generic;
using UnityEngine;

public class FlockingState : EnemyInteractionState
{ 
    Vector3 smoothedAvoidance;
    
    public FlockingState(EnemyContext context, EnemyStateMachine.EnemyStates estate) : base(context, estate)
    {

    }
    public override void EnterState()
    {
        Context.IsFlocking = true;
        SpacialHashMap.instance.Register(Context);

        //Debug.Log("Entered FlockingState");
    }
    public override void UpdateState()
    {
      
    }

    public override void ExitState()
    {
        SpacialHashMap.instance.UnRegister(Context);
    }
    public override void FixedUpdateState()
    {
        SpacialHashMap.instance.GetNeighbours(Context.EnemyTransform.position, myListOfNeighbours);
        Vector3 rawAvoidance = ObstacleAvoidance(10f, 30f);
        smoothedAvoidance = Vector3.Lerp(smoothedAvoidance, rawAvoidance, Time.deltaTime * .8f);
        Vector3 flocking =
            Alignment(myListOfNeighbours) +
            Cohesion(myListOfNeighbours) +
            Separation(myListOfNeighbours) +
            smoothedAvoidance;

        Vector3 velocity = Vector3.zero;
        if (flocking != Vector3.zero)
        {
            Context.EnemyTransform.rotation = Quaternion.Slerp(Context.EnemyTransform.rotation, Quaternion.LookRotation(flocking), Time.fixedDeltaTime * Context.TurnSpeed);
            velocity = Vector3.ClampMagnitude(flocking * Context.Speed - Context.Rb.linearVelocity, Context.MaxForce);
        }
        Context.Rb.AddForce(velocity);
    }

    public override void OnTriggerEnter(Collider other) 
    {

    }
    public override void OnTriggerStay(Collider other) 
    { 

    }
    public override void OnTriggerExit(Collider other) 
    { 

    }

    public override EnemyStateMachine.EnemyStates GetNextState()
    {
        if ( Context.HealthSystem.HasTakenDamage() || Context.IsSeeking)
        {
            return EnemyStateMachine.EnemyStates.Seek;
        }
        return StateKey;
    }

    Vector3 Alignment(List<EnemyContext> neighbour)
    {
        Vector3 total = Vector3.zero;
        int count = 0;

        foreach (EnemyContext other in neighbour)
        {
            if (other == base.Context || other.IsSloted || other.EnemyTransform == null)
                continue;

            total += other.EnemyTransform.forward;
            count++;
        }

        if (count == 0)
            return ChangeDirection();

        Vector3 average = total / count;
        
        return average.normalized * Context.AlignmentWeight;
    }

    Vector3 ChangeDirection()
    {
        Vector3 direction = Context.EnemyTransform.forward;
        float turnAmount = Mathf.Sin(Time.time * 0.8f) * 0.6f;

        direction += Context.EnemyTransform.right * turnAmount;

        return direction.normalized * Context.AlignmentWeight;
    }

    Vector3 Cohesion(List<EnemyContext> neighbour)
    {
        if (neighbour.Count <= 1)
        {
            return Vector3.zero;
        }
        
        Vector3 total = Vector3.zero;
        int count = 0;

        foreach (EnemyContext other in neighbour)
        {
            if (other == base.Context || other.IsSloted || other.EnemyTransform == null)
                continue;

            total += other.EnemyTransform.position;
            count++;
        }

        if (count == 0)
            return Vector3.zero;

        Vector3 average = total / count;

        Vector3 cohesion = (average - Context.EnemyTransform.position);

        return cohesion.normalized * Context.CohesionWeight;
    }

    new Vector3 Separation(List<EnemyContext> neighbours)
    {

        if (neighbours.Count <= 1)
        {
            return Vector3.zero;
        }


        Vector3 SeparationAngle = Vector3.zero;


        foreach (EnemyContext other in neighbours)
        {
            if (other == base.Context || other.IsSloted || other.EnemyTransform == null)
                continue;

            float distance = Vector3.Distance(base.Context.EnemyTransform.position, other.EnemyTransform.position);

            if (distance < base.Context.SeparationDistance)
            {
                Vector3 distanceBetween = base.Context.EnemyTransform.position - other.EnemyTransform.position;

                Vector3 pushAway = distanceBetween.normalized / distance;

                SeparationAngle += pushAway;
            }
        }

        return SeparationAngle.normalized * Context.SeparationWeight;
    }
}
