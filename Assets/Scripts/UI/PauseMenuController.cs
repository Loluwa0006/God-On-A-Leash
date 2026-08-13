using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;

    private void Start()
    {
        pauseScreen.SetActive(false);
    }
    public void PauseGame()
    {
        EntityManager.Instance.PauseGame();
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        EntityManager.Instance.UnpauseGame();
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void TogglePause()
    {
        if (EntityManager.Instance.GamePaused)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        //Need game to restart
        Destroy(EntityManager.Instance); 
        SceneManager.LoadScene("MainMenu");
    }
}
