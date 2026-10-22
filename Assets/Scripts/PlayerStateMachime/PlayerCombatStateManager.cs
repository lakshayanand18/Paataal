using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerCombatStateManager : StateManager<PlayerCombatStateManager.CombatStates>
{
    public enum CombatStates
    {
        Rotate,
        DeadZone,
        DeadZoneRotate
    }
    
    //Rotation
    [SerializeField] private float deadzoneRadius = 100f;
    [SerializeField] private float rotationSpeed = 0.07f;
    [SerializeField] private float localModelRotationSpeed;
    [SerializeField] private float localModelOffsetSpeed = 1.5f;
    //crosshair
    [Tooltip("Add crosshair for the ship")]
    [SerializeField] private RectTransform aim;
    [SerializeField] private CrossMovement crosshair;
    
    // Particle System
    [SerializeField] private ParticleSystem[] lasers;
    [SerializeField] private float fireRate = 1f;

    //Movement
    [SerializeField] private Transform playerModel;
    private Rigidbody _rb;
    public PlayerInput PlayerInput; 

    //health
    [SerializeField] private int health = 100;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int damage;
    private HealthSystem _playerHealth;
    
    //firing logic
    bool _isFiring;
    Camera  _mainCamera;
    Coroutine _fireCoroutine;
    [SerializeField] ParticleSystem[] laserParticles;

    private PlayerContext _context;
    
    protected IEnumerator FireCoroutine()
    {
        while(true)
        {
            if (_isFiring)
            {
                foreach (ParticleSystem laser in laserParticles)
                {
                    laser.transform.LookAt(GetAimTarget());
                    laser.Emit(1);
                }

                yield return new WaitForSeconds(fireRate);
            }

            yield return null;
        }
    }
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        //InputStart
        PlayerInput = new PlayerInput();
        PlayerInput.CharacterControls.fire.started += OnFireInput;
        PlayerInput.CharacterControls.fire.canceled += OnFireInput;

        //health System
        _playerHealth = new HealthSystem(health);
        healthBar.Setup(_playerHealth);

        _context = new PlayerContext();

        _context.Rigidbody = _rb;
        _context.MainCamera = Camera.main;
        _context.DeadzoneRadius = deadzoneRadius;
        _context.Crosshair = crosshair;
        _context.ModelOffsetSpeed = localModelOffsetSpeed;
        _context.PlayerModelRotationSpeed = localModelRotationSpeed;
        _context.RotationSpeed = rotationSpeed;
        _context.PlayerModel = playerModel;

        InitializeState();
        
        if(_fireCoroutine == null)
            _fireCoroutine = StartCoroutine(FireCoroutine());
    }
    
    void OnFireInput(InputAction.CallbackContext context)
    {
        _isFiring = context.ReadValueAsButton();
    }

    void InitializeState()
    {
        States.Add(CombatStates.Rotate, new PlayerRotationState(_context, CombatStates.Rotate));
        States.Add(CombatStates.DeadZone, new PlayerDeadZoneState(_context, CombatStates.DeadZone));
        States.Add(CombatStates.DeadZoneRotate, new PlayerDeadZoneRotationState(_context, CombatStates.DeadZoneRotate));
        CurrentState = States[CombatStates.DeadZone];
    }
    
    // casts a ray cast to the crosshair
    Vector3 GetAimTarget() 
    {
        Ray ray = _mainCamera.ScreenPointToRay(aim.position);
        if (Physics.Raycast(ray, out RaycastHit cameraHit))
        {
            return cameraHit.point;
        }
        return ray.origin + ray.direction * 1000f;
    }
    
    public void PlayerDamage(int damage)
    {
        _playerHealth.Damage(damage);
    }
    
    public bool IsPlayerDead(bool dead)
    {
        if (_playerHealth.GetHealthPercentage() == 0f)
        {
            dead = true;

            if (_fireCoroutine != null)
            {
                StopCoroutine(_fireCoroutine);
            }
        }
        else
            dead = false;
        return dead;    
    }
    
    void OnEnable() {
        PlayerInput.CharacterControls.Enable();
    }
    void OnDisable() {
        PlayerInput.CharacterControls.Disable();
    }
}