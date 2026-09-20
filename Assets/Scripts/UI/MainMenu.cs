using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        currentScene++;
        SceneManager.LoadScene(currentScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
