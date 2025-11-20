using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Apenas necessário para sair do PlayMode no editor
#endif

public class ManagerMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;
    // Propriedade helper para verificar se o menu existe nesta cena
    public bool HasPauseMenu => pauseMenu != null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (HasPauseMenu)
        {
            HidePauseMenu();
        }
        else
        {
            // Garante que o jogo não fique congelado caso a escala tenha sido alterada em outra cena
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Se a cena não tem pauseMenu, ignoramos totalmente a tecla ESC
        if (!HasPauseMenu) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }
    
    public void PlayGame() // PlayGame na verdade leva para o interludio 1
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayGameInterlude() // Leva para o jogo - Mapa 1
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void PlayGameInterlude2() // Leva para o jogo - Mapa 2
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

    public void ShowPauseMenu()
    {
        if (!HasPauseMenu) return; // Segurança extra
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        if (!HasPauseMenu)
        {
            // Mesmo sem menu, garantimos que o tempo esteja rodando
            Time.timeScale = 1f;
            isPaused = false;
            return;
        }
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
