using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyInteractionState : BaseState<EnemyStateMachine.EnemyStates>
{
    protected EnemyContext Context;
    protected List<EnemyContext> myListOfNeighbours = new List<EnemyContext>(50);

    public EnemyInteractionState(EnemyContext context, EnemyStateMachine.EnemyStates stateKey) : base(stateKey)
    {
        Context = context;
    }

    public List<EnemyContext> Enemies()
    {
        return myListOfNeighbours;
    }

    public Vector3 ObstacleAvoidance(float radius, float maxDistance)
    {
        Vector3 totalAvoidanceForce = Vector3.zero;

        // 1. Establish the base forward vector from the nose
        Vector3 forward = Context.EnemyTransform.forward;

        // 2. Build the 45-degree cone using LOCAL axes (handles pitch, yaw, and roll perfectly)
        Vector3 forwardLeft = Quaternion.AngleAxis(-45, Context.EnemyTransform.up) * forward;
        Vector3 forwardRight = Quaternion.AngleAxis(45, Context.EnemyTransform.up) * forward;
        Vector3 forwardUp = Quaternion.AngleAxis(-45, Context.EnemyTransform.right) * forward;
        Vector3 forwardDown = Quaternion.AngleAxis(45, Context.EnemyTransform.right) * forward;

        // 3. Grab the pure 90-degree flanks directly from the transform axes
        Vector3 pureLeft = -Context.EnemyTransform.right;
        Vector3 pureRight = Context.EnemyTransform.right;
        Vector3 pureUp = Context.EnemyTransform.up;
        Vector3 pureDown = -Context.EnemyTransform.up;

        Vector3 backLeft = Quaternion.AngleAxis(-45, Context.EnemyTransform.up) * -forward;
        Vector3 backRight = Quaternion.AngleAxis(45, Context.EnemyTransform.up) * -forward;
        Vector3 backUp = Quaternion.AngleAxis(-45, Context.EnemyTransform.right) * -forward;
        Vector3 backDown = Quaternion.AngleAxis(45, Context.EnemyTransform.right) * -forward;

        Vector3[] rays = {forward, pureUp, pureLeft, 
        forwardLeft, forwardRight, forwardUp, forwardDown, 
        backUp, backDown, backLeft, backRight, pureRight, pureDown };

        
        float[] danger = new float[13];

        for (int i = 0; i < 13; i++)
        {
            RaycastHit hit;
            if (Physics.SphereCast(Context.EnemyTransform.position, radius, rays[i], out hit, maxDistance, Context.ObstacleLayerMask))
            {
                // Context Steering Danger Score (0.0 to 1.0)
                danger[i] = 1f - (hit.distance / maxDistance);

                Vector3 Direction = - (hit.point - Context.EnemyTransform.position);
                totalAvoidanceForce += Direction.normalized * danger[i];

            //Debug
                Debug.DrawRay(Context.EnemyTransform.position, rays[i] * danger[i] * 50f, Color.red);
            }
            else
            {
                danger[i] = 0f;
            }

        }

        return totalAvoidanceForce * Context.AlignmentWeight;
    }

    public void HordeCheck()
    {
        SpacialHashMap.instance.GetNeighbours(Context.EnemyTransform.position, myListOfNeighbours);
        foreach (EnemyContext enemy in myListOfNeighbours)
        { 
            if(enemy.IsSeeking == true)
            {
                Context.IsSeeking = true;
            }
        }
    }

    public Vector3 Separation(List<EnemyContext> neighbours)
    {

        if (Context.IsLeader || neighbours.Count <= 1)
        {
            return Vector3.zero;
        }


        Vector3 separationValue = Vector3.zero;


        foreach (EnemyContext other in neighbours)
        {
            if (other == this.Context || other.EnemyTransform == null)
                continue;

            float distance = Vector3.Distance(this.Context.EnemyTransform.position,
                             other.EnemyTransform.position);

            if (distance < this.Context.SeparationDistance)
            {
                Vector3 distanceBetween = this.Context.EnemyTransform.position
                                         - other.EnemyTransform.position;

                Vector3 pushAway = distanceBetween.normalized / distance;

                separationValue += pushAway;
            }
        }

        return separationValue.normalized * Context.SeparationWeight;
    }
}