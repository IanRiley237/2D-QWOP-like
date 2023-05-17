using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static bool gameIsPaused;
    public GameObject pauseMenu;

    void TogglePause(InputAction.CallbackContext context)
    {
        gameIsPaused = !gameIsPaused;
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        TogglePause(context);
    }
}
