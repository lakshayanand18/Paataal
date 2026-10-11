using System.Collections.Generic;
using UnityEngine;

public class DeathState : EnemyInteractionState
{
    public DeathState(EnemyContext context, EnemyStateMachine.EnemyStates estate) : base(context, estate)
    {
        this.Context = context;

    }

    public override void EnterState()
    {
        // Detach the particle for parent
        Context.HeartParticles.transform.SetParent(null);
        Context.DeathParticles.transform.SetParent(null);
        
        // Release the slot
        AttackSlotManager.instance.ReleaseSlot(this.Context);
        
        //UnSlot the Spacial Hash Map and Enemy UI
        EnemyUI.instance.UnRegister(Context.EnemyTransform);
        SpacialHashMap.instance.UnRegister(Context);
        
        //blood
        Context.DeathParticles.Play();
        Context.HeartParticles.Play();

        //finally Distroy the player
        UnityEngine.Object.Destroy(Context.MonoBehaviour.gameObject, 4f);
        UnityEngine.Object.Destroy(Context.DeathParticles.gameObject, Context.DeathParticles.main.duration + Context.DeathParticles.main.startLifetime.constantMax);
        UnityEngine.Object.Destroy(Context.HeartParticles.gameObject, Context.HeartParticles.main.duration + Context.HeartParticles.main.startLifetime.constantMax);
    }

    public override void ExitState() { }
    public override void UpdateState()
    {
        if (Context.EnemyTransform.rotation != Quaternion.Euler(Vector3.right * 90f))
        {
            Context.Rb.AddRelativeForce((Vector3.down * 10f) + (Vector3.forward * 20f));
            Context.Rb.AddRelativeTorque(Vector3.right * 10f);
        }
    }
    public override void FixedUpdateState() { }

    public override EnemyStateMachine.EnemyStates GetNextState() 
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}

