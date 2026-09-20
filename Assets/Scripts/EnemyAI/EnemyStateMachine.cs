using UnityEngine;

public class EnemyStateMachine : StateManager<EnemyStateMachine.EnemyStates>
{
    public enum EnemyStates
    {
        Flocking,
        Attack,
        Seek
    }
    
    //fish health System
    [SerializeField] int health;
    private  HealthSystem healthSystem;
    bool _leader;
    Rigidbody _rb;
    
    //Player particle collision 
    [SerializeField] private ParticleSystem[] lasers;
    [SerializeField] PlayerStateMachine playerStateMachine;
    [SerializeField] Transform player;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] LayerMask obstacleLayerMask;
    [SerializeField] Collider playerCollider;
    private EnemyContext _context;
    
    // Movement
    public float maxForce; 
    public float speed = 2f;
    public float turnSpeed;
    
    [Header ("Flocking behaviour")]
    [SerializeField, Range(0f, 5f)] float separationWeight; // force of separation
    [SerializeField, Range(0f, 5f)] float cohesionWeight; // force of cohesion
    [SerializeField, Range(0f, 5f)] float alignmentWeight; // force of alignment
    [SerializeField, Range(0f, 10f)] float avoidanceWeight; // force if avoidance
    [SerializeField] float maxFlockingDistanceFromPlayer = 90f;
    [SerializeField, Range(0f, 5f)] float returnToPlayerWeight = 2.5f;
    public float separationDistance = 10f;

    // flags
    bool isSeeking;
    bool isSeekingPlayer;
    
    void Awake()
    {
        // Rigid Body
        _rb = GetComponent<Rigidbody>();

        // Health System
        healthSystem = new HealthSystem(health);
        
        if (playerStateMachine == null)
        {
            playerStateMachine = FindAnyObjectByType<PlayerStateMachine>();
            player = playerStateMachine.transform;
            playerCollider = playerStateMachine.GetComponent<Collider>();
        }
        
        // Enemy Context constructor 
        _context = new EnemyContext
            (_rb,
            player,
            maxForce,
            playerCollider,
            transform,
            speed,
            turnSpeed,
            lasers,
            playerLayerMask,
            obstacleLayerMask,
            _leader,
            this,
            separationDistance,
            separationWeight,
            cohesionWeight,
            alignmentWeight,
            avoidanceWeight,
            maxFlockingDistanceFromPlayer,
            returnToPlayerWeight,
            health,
            healthSystem,
            isSeeking,
            isSeekingPlayer,
            playerStateMachine
            );

        InitializeStates();
    }

    protected override void Start()
    {
        base.Start();
        SpacialHashMap.instance.Register(_context);

        EnemyUI.instance.Register(transform, healthSystem);
    }
    protected override void Update()
    {
        base.Update();
    }
    private void InitializeStates()
    {
        States.Add(EnemyStates.Seek, new SeekState(_context, EnemyStates.Seek));
        States.Add(EnemyStates.Attack, new AttackState(_context, EnemyStates.Attack));
        States.Add(EnemyStates.Flocking, new FlockingState(_context, EnemyStates.Flocking));
        CurrentState = States[EnemyStates.Seek];
    }


    public void OnParticleCollision(GameObject other)
    {
        healthSystem.Damage(10);


        if (healthSystem.GetHealthPercentage() == 0)
        {
            AttackSlotManager.instance.ReleaseSlot(_context);
            EnemyUI.instance.UnRegister(transform);
            SpacialHashMap.instance.UnRegister(_context);
            Destroy(this.gameObject);
        }
    }

    //private void OnDrawGizmos()
    //{
        //if (_context.IsLeader)
        //{
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawCube(transform.position, Vector3.one * 20f);
        //}
        //else return;


        //foreach (var key in AttackSlotManager.slots)
        //{
        //    Vector3 slotPos = AttackSlotManager.instance.GetSlotWorldPosition(key.Key, player);

        //    Gizmos.color = key.Value ? Color.green : Color.red;
        //    Gizmos.DrawSphere(slotPos, 10f);
        //}

        //if (_context.IsSloted)
        //{
        //    Gizmos.DrawSphere(transform.position, 10f);
        //}

        //else return;
    //}
}

