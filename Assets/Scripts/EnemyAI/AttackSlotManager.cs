using System.Collections.Generic;
using UnityEngine;

public class AttackSlotManager : MonoBehaviour
{
    public static Dictionary<int, bool> slots = new Dictionary<int, bool>();
    private static Vector3[] slotOffsetsList;
    [SerializeField] private Transform player;
    [SerializeField] private float radius = 50f;

    public static AttackSlotManager instance;

    // queue of enemies waiting for a slot
    private Queue<EnemyContext> waitingEnemies = new Queue<EnemyContext>();

    private void Awake()
    {
        instance = this;
        slots.Clear();
        slotOffsetsList = null;
    }

    void Start()
    {
        BuildSlots();
    }
    
    public void BuildSlots()
    {
        if (slotOffsetsList != null)
        {
            return;
        }

        List<Vector3> offsets = new List<Vector3>();

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    offsets.Add(new Vector3(x, y, z).normalized);
                }

        slotOffsetsList = offsets.ToArray();

        for (int i = 0; i < slotOffsetsList.Length; i++)
            slots[i] = false;
    }

    // enemy calls this when entering SeekState
    public void RequestSlot(EnemyContext context)
    {
        // already has a slot, no need to request
        if (context.IsSloted)
            return;

        int claimedSlot;
        if (GetAvailableSlot(out claimedSlot))
        {
            // assign slot
            AssignSlot(context, claimedSlot);
        }
        else
        {
            // no slots free — add to queue and send to flock
            if (!waitingEnemies.Contains(context))
                waitingEnemies.Enqueue(context);
            context.IsFlocking = true;
        }
    }

    // assign a slot index to an enemy
    private void AssignSlot(EnemyContext context, int slotIndex)
    {
        slots[slotIndex] = true;
        context.AssignedSlotIndex = slotIndex;
        context.IsSloted = true;
        context.IsFlocking = false;
    }

    // convert index to world position
    public Vector3 GetSlotWorldPosition(int index, Transform player)
    {
        return player.position + slotOffsetsList[index] * radius;
    }

    private bool GetAvailableSlot(out int claimedSlot)
    {
        foreach (var key in slots.Keys)
        {
            if (!slots[key])
            {
                claimedSlot = key;
                return true;
            }
        }

        claimedSlot = -1;
        return false;
    }

    // called when enemy dies or leaves slot 
    public void ReleaseSlot(EnemyContext context)
    {
        if (context.IsSloted && slots.ContainsKey(context.AssignedSlotIndex))
        {
            slots[context.AssignedSlotIndex] = false;
            context.IsSloted = false;
            context.AssignedSlotIndex = -1;

            // assign freed slot to next waiting enemy
            if (waitingEnemies.Count > 0)
            {
                EnemyContext next = waitingEnemies.Dequeue();
                int newSlot;
                if (GetAvailableSlot(out newSlot))
                {
                    AssignSlot(next, newSlot);
                    next.IsSeeking = true; // pull out of flocking back to seek
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
{
    if (slotOffsetsList == null || player == null)
        return;

    Gizmos.color = Color.green;

    for (int i = 0; i < slotOffsetsList.Length; i++)
    {
        Vector3 slotPos = player.position + slotOffsetsList[i] * radius;
        Gizmos.DrawWireSphere(slotPos, 1f);
    }
}
}