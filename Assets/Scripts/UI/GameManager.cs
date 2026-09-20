using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header ("Player")]
    [SerializeField] PlayerStateMachine player;
    [SerializeField] CrossMovement Crosshair;

    [Header("Game Over")]
    [SerializeField] GameOver gameOver;
    
    [Header("Winner")]
    [SerializeField] WinDisplay winner;

    [Header("Pause")]
    [SerializeField] Pause pause;
    
    [Header("TimeWarning")]
    [SerializeField] TimeStartWarning timeStartWarning;
    
    [Header("TimeLine")]
    [SerializeField] TimeLine timeLine;

    bool playerDeath = false;
    bool _isPaused;

    void Start()
    {
        player._playerInput.CharacterControls.Pause.started += onPauseInput;
        player._playerInput.CharacterControls.Pause.canceled += onPauseInput;
    }

    void onPauseInput(InputAction.CallbackContext context)
    {
        _isPaused = context.ReadValueAsButton();
    }
    
    private void Update()
    {
        timeStartWarning.TimeWarning();
        timeLine.TimeDisplay();

        if (timeLine.TimeShown <= 0)
        {
            Crosshair.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            winner.Setup();
        }
        if (player.IsPlayerDead(playerDeath))
        {
            Crosshair.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameOver.Setup();
        }

        if(_isPaused)
        {
            Crosshair.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pause.Setup();
        }
    }
}