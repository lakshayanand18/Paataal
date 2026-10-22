using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContext 
{
    private Rigidbody _rb;
    private float _movementSpeed;
    private Vector3 _currentMovementInput;
    private float _movementSpeedModifier;
    private bool _isMovementPressed;
    private bool _isSpeedPressed;
    
    public Rigidbody Rigidbody { get; set; }

    public float MovementSpeed
    {
        get => _movementSpeed;
        set => _movementSpeed = value;
    }

    public float MovementSpeedModifier
    {
        get => _movementSpeedModifier; 
        set => _movementSpeedModifier = value;
    }

    public bool IsMovementPressed
    {
        get => _isMovementPressed; 
        set => _isMovementPressed = value;
    }

    public Vector3 MovementInput
    {
        get => _currentMovementInput; 
        set => _currentMovementInput = value;
    }

    public bool IsSpeedPressed
    {
        get => _isSpeedPressed;
        set => _isSpeedPressed = value;
    }
    
    public Camera MainCamera { get;  set; }
    public RectTransform Aim { get;  set; }
    public ParticleSystem[] Lasers {  get; set; }
    public Transform PlayerModel { get; set; }
    public float PlayerModelRotationSpeed { get; set; }
    public float ModelOffsetSpeed { get; set; }
    public float RotationSpeed { get; set; }
    public float DeadzoneRadius { get; set; }
    public CrossMovement Crosshair { get; set; }
}
