using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedMenu : MonoBehaviour
{
    //public string mainMenuLevel;
    public GameManager gm;

    public void RestartGame()
    {
        gm.Reset();
    }

    public void QuitToMain()
    {
        gm.activateMainMenu();
    }
}
