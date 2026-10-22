using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementStateManager : StateManager<PlayerMovementStateManager.MovementStates>
{
   public enum MovementStates
   {
      Idle,
      Move,
      Speed
   }
   [SerializeField] float movementSpeed = 1000f;
   [SerializeField] float movementSpeedModifier = 1.5f;
   private Rigidbody _rb;
   private Vector3 _smoothPos;
   private PlayerInput _playerInput; // this is declaired in the inspector

   //variable to store player input values
   Vector3 _currentMovementInput;
   Vector3 _currentMovement;


   //bool modifiers
   bool _isMovementPressed = false;
   bool _isSpeedPressed = false;
   bool _isFiring = false;


   private PlayerContext _context;

   
   void Awake()
   {
      _rb = GetComponent<Rigidbody>();

      _playerInput = new PlayerInput();
      // Movement Input
      _playerInput.CharacterControls.Move.started += OnMovementInput;
      _playerInput.CharacterControls.Move.canceled += OnMovementInput;
      _playerInput.CharacterControls.Move.performed += OnMovementInput;

      //Speed Input
      _playerInput.CharacterControls.Speed.started += OnSpeedInput;
      _playerInput.CharacterControls.Speed.canceled += OnSpeedInput;

      _context = new PlayerContext();
      _context.Rigidbody = _rb;

      InitializeStates();
   }

   void InitializeStates()
   {
      States.Add(MovementStates.Idle, new PlayerIdleState(_context, MovementStates.Idle));
      States.Add(MovementStates.Move, new PlayerMoveState(_context, MovementStates.Move));
      States.Add(MovementStates.Speed, new PlayerSpeedState(_context, MovementStates.Speed));
      CurrentState = States[MovementStates.Idle];
   }
   // movement Callback
   void OnMovementInput(InputAction.CallbackContext context)
   { 
      _context.MovementInput = context.ReadValue<Vector3>(); 
      _context.IsMovementPressed = _context.MovementInput != Vector3.zero;
   }
   
   
   // Speed Callback
   void OnSpeedInput(InputAction.CallbackContext context) 
   {
      _context.IsSpeedPressed = context.ReadValueAsButton();
   }
   
   void OnEnable() 
   {
      _playerInput.CharacterControls.Enable();
   }
   
   void OnDisable() 
   {
      _playerInput.CharacterControls.Disable();
   }
}