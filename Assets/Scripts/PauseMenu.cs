using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //fields for menu and gamePausing
    public static bool GameIsPaused = false;
    public GameObject pauseMenu;

    //deactivates pauseMenu at start, game resumes
    private void Start()
    {
        pauseMenu.SetActive(false);
        Resume();
    }

    //check for escape key, adjust accordingly
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if paused, resume now
            if (GameIsPaused)
            {
                Resume();
            }
            //else, pause now
            else
            {
                Pause();
            }
        }
    }

    //pauses game
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    //resumes game
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    //restarts game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        Destroy(Data.S.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //go to main menu
    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        Destroy(Data.S.gameObject);
        SceneManager.LoadScene(0);
    }
}