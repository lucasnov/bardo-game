using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    // Defina aqui o índice ou o nome da cena do menu
    public int menuSceneIndex = 0;
    // ou se preferir: public string menuSceneName = "MainMenu";

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
        // ou: SceneManager.LoadScene(menuSceneName);
    }
}
