using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsMenu : MonoBehaviour
{
    public GameManager gm;

    public void Back()
    {
        gm.activateMainMenu();
    }

    public void Level1()
    {
        gm.L1();
    }

    public void Level2()
    {
        gm.L2();
    }

    public void Level3()
    {
        gm.L3();
    }
}
