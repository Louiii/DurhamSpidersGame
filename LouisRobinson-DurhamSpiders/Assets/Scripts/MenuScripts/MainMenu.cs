using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameManager gm;

    public void Play()
    {
        gm.L1();
    }

    public void Levels()
    {
        gm.OpenLevelsMenu();
    }
}
