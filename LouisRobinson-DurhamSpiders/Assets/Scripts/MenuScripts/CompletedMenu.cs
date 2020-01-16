using UnityEngine;
using UnityEngine.UI;

public class CompletedMenu : MonoBehaviour
{
    //public string mainMenuLevel;
    public GameManager gm;
    public Text text;

    public void RestartGame()
    {
        gm.Reset();
    }

    public void QuitToMain()
    {
        gm.activateMainMenu();
    }

    public void NextLevel()
    {
        gm.NextLevel();
    }
}
