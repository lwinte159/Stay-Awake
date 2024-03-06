using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //starts game
    public void StartGame()
    {
        Destroy(Data.S);
        SceneManager.LoadScene(1);
    }

    //exits game
    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Quit if in editor
        #endif
    }
}
