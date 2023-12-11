using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gamePausedUI;
  
    bool gamePaused;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverUI.activeInHierarchy || gamePausedUI.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void quit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        gamePaused = false;
        gamePausedUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ResumeWithInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!gamePaused)
            {
               
                gamePaused = true;
                gamePausedUI.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                gamePaused = false;
                gamePausedUI.SetActive(false);
                Time.timeScale = 1f;
            }

        }
       
    }
}

