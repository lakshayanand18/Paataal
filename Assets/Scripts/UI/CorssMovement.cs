using UnityEngine;
public class CrossMovement : MonoBehaviour
{
    [SerializeField] Vector2 initialPos;
    [SerializeField] RectTransform crosshair; // Move crosshair
    [SerializeField] float clampFactor = 150f;
    [SerializeField] float sensitivity = 1f;
    public Vector2 lookDelta;
    public Vector2 offset;
    PlayerInput playerInput;

    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.CharacterControls.Look.performed += ctx =>
        {
            lookDelta = ctx.ReadValue<Vector2>();
        };
        initialPos = crosshair.anchoredPosition;
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void Update()
    {
        MoveCrosshair();
        lookDelta = Vector2.zero;
        
    }
    void MoveCrosshair()
    {   
        offset += lookDelta * sensitivity;
        offset = Vector2.ClampMagnitude(offset, clampFactor);
        crosshair.anchoredPosition = initialPos + offset;
    }
}
