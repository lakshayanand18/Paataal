using UnityEngine;

public class Pause : MonoBehaviour
{
    public CrossMovement Crosshair;
    public void Setup()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Crosshair.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
