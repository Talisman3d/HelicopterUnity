using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{

    public static bool isPaused = false;

    [SerializeField] InputAction gamePauseButton;
    [SerializeField] GameObject pauseMenuUI;

    private void OnEnable() {
        gamePauseButton.Enable();
        gamePauseButton.performed += FlipPauseBool; // When the reset button is pressed, do this function
    }

    private void FlipPauseBool(InputAction.CallbackContext context){
        isPaused=!isPaused;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPaused){
            ResumeGame();
        }
        else{
            PauseGame();
        }
    }

    void ResumeGame(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void PauseGame(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
