using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStateMachine : MonoBehaviour
{

    //Rotation
    [SerializeField]float _deadzoneRadius = 100f;
    [SerializeField] float _rotationSensitivity = 0.07f;
    [SerializeField] float _localRotationSpeed = 2f;
    [SerializeField] float _localCameraMovement;
    [SerializeField] CrossMovement _crosshair;  //crosshair script
    [SerializeField] RectTransform _aim;
    [SerializeField] RectTransform UI;
    Vector2 offset;

    //Aim
    RaycastHit cameraHit;
    RaycastHit playerHit;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject[] lazers;
    ParticleSystem emmitionSystem;

    //Movement
    [SerializeField] float _performenceStateSpeed;
    [SerializeField] float _movementSpeed = 1000f;
    [SerializeField] float _movementSpeedModifier = 1f;
    [SerializeField] Transform _playerModel;
    Rigidbody _rb;
    private Vector3 _moveDirection;
    private Vector3 _smoothPos;
    public PlayerInput _playerInput;   // this is declaired in the inspector

    //variable to store player input values
    Vector3 _currentMovementInput;
    Vector3 _currentMovement;
    Vector2 _currentAbilityType;
    // variables for coroutines 

    //EnemyAttackSloting
    AttackSlotManager _attackSlotManager ;

    //health
    [SerializeField] int health = 100;
    [SerializeField] HealthBar healthBar;
    [SerializeField] int damage;
    HealthSystem playerHealth;


    //bool check
    bool _isPerformenceAndCombatStatePressed = false;
    bool _isMovementPressed = false;
    bool _isSpeedPressed = false;
    bool _isFiring = false;

    //State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    //getter setter
    public Vector3 CurrentMovement { get { return _currentMovement; } }
    public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    public Vector3 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }
    public Rigidbody Rb {get { return _rb; } set { _rb = value; }}
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public float PerformenceStateSpeed { get { return _performenceStateSpeed; } set { _performenceStateSpeed = value; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsSpeedPressed { get { return _isSpeedPressed; } }
    public bool IsPerformenceAndCombatPressed { get{ return _isPerformenceAndCombatStatePressed; } }
    public bool IsSpawnProtection { get { return _isSpawnProtection; } }
    public Vector2 CurrentAbilityType { get { return _currentAbilityType; } }
    public Vector3 SmoothPos { get { return _smoothPos; } set { _smoothPos = value; } }
    public float MovementSpeedModifier { get { return _movementSpeedModifier; } set { _movementSpeedModifier = value; } }
    //Spawn Protection
    bool _isSpawnProtection = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _performenceStateSpeed = 1.3f;
        //InputState
        _playerInput = new PlayerInput();
         _playerInput.CharacterControls.fire.started += onFireInput;
        _playerInput.CharacterControls.fire.canceled += onFireInput;
        _playerInput.CharacterControls.Move.started += onMovementInput;
        _playerInput.CharacterControls.Move.canceled += onMovementInput;
        _playerInput.CharacterControls.Move.performed += onMovementInput;
        _playerInput.CharacterControls.Speed.started += onSpeedInput;
        _playerInput.CharacterControls.Speed.canceled += onSpeedInput;
        _playerInput.CharacterControls.PerformenceAndCombatState.started += onPerformenceStateInput;
        _playerInput.CharacterControls.PerformenceAndCombatState.canceled += onPerformenceStateInput;

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Combat();
        _currentState.EnterState();

        //health System
        playerHealth = new HealthSystem(health);
        healthBar.Setup(playerHealth);

    }

    void Start()
    {
        mainCamera = Camera.main;
    }
    void onFireInput(InputAction.CallbackContext context)
    {
        _isFiring =context.ReadValueAsButton();
    }
    void onPerformenceStateInput(InputAction.CallbackContext context)
    {
        _isPerformenceAndCombatStatePressed = context.ReadValueAsButton();
    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector3>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0 || _currentMovementInput.z != 0;
    }
    void onSpeedInput(InputAction.CallbackContext context) 
    {
        _isSpeedPressed = context.ReadValueAsButton();
    }
    void Update() 
    {
        _currentState.UpdateStates();

        //PlayerDamage(5);
    }
    void FixedUpdate() {
        ProcessMovement();
        ProcessRotation();
        ModelBehaviour();
        Processfire();
    }
    void ProcessRotation() {
        //rotate player model to face movement direction
        offset = _crosshair.offset;

        Quaternion TargetRotation = Quaternion.Euler(-offset.y / 530f * 45f, offset.x / 530f * 45f, -offset.x / 530f * 30f);
        _playerModel.localRotation = Quaternion.Slerp(_playerModel.localRotation, TargetRotation, Time.fixedDeltaTime * _localRotationSpeed); 

        if(offset.sqrMagnitude >= _deadzoneRadius * _deadzoneRadius) {
            Vector3 torque = new Vector3(-offset.y, offset.x, 0f) * _rotationSensitivity;   
            _rb.AddRelativeTorque(torque, ForceMode.Force);  
        }
        else if(_crosshair.offset.sqrMagnitude < _deadzoneRadius * _deadzoneRadius && _crosshair.lookDelta != Vector2.zero) {
            Vector3 torque = new Vector3(-offset.y, offset.x, 0f) * _rotationSensitivity;                                                                       
            _rb.AddRelativeTorque(torque, ForceMode.Force);
        }
        else if (_crosshair.offset.sqrMagnitude < _deadzoneRadius * _deadzoneRadius && _crosshair.lookDelta == Vector2.zero) {
            _rb.AddForce(0f, 0f, 0f);
        }
    }
    void ProcessMovement() 
    {
        _rb.AddRelativeForce(_currentMovementInput * _movementSpeed * _movementSpeedModifier * Time.fixedDeltaTime);
    }
    void ModelBehaviour() 
    {
        Vector3 offset2D = new Vector3(-offset.x /530f * 10f,-offset.y / 530f * 8f, 0f);
        _playerModel.localPosition = Vector3.Lerp(_playerModel.localPosition, offset2D, Time.fixedDeltaTime * _localCameraMovement);
    }

    void Processfire() {
        foreach(GameObject lazer in lazers) 
        {
            Vector3 aim = GetAimTarget();
            lazer.transform.LookAt(aim);
            var emmitionSystem = lazer.GetComponent<ParticleSystem>().emission;
            emmitionSystem.enabled = _isFiring;
        }
    }
    public Vector3 GetAimTarget() {
        Ray ray = mainCamera.ScreenPointToRay(_aim.position);
        if (Physics.Raycast(ray, out cameraHit))
        {
            return cameraHit.point;
        }
        
        return ray.origin + ray.direction * 1000f;
    }

    public void PlayerDamage(int damage)
    {
        playerHealth.Damage(damage);
    }

    public bool IsPlayerDead(bool dead)
    {
        if (playerHealth.GetHealthPercentage() == 0f)
        {
            dead = true;
        }
        else
            dead = false;
        return dead;    
    }

    void OnEnable() {
        _playerInput.CharacterControls.Enable();
    }
    void OnDisable() {
        _playerInput.CharacterControls.Disable();
    }
}