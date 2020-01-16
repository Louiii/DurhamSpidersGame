using UnityEngine;

public class DisplayLives : MonoBehaviour
{
    private GUIStyle livesStyle = new GUIStyle();
    private GUIStyle powerStyle = new GUIStyle();
    private GUIStyle treasureStyle = new GUIStyle();
    private GUIStyle levelStyle = new GUIStyle();

    public GameManager gm;

    void Start()
    {
        livesStyle.fontSize = 50;
        livesStyle.normal.textColor = Color.red;

        powerStyle.fontSize = 50;
        powerStyle.normal.textColor = Color.magenta;

        treasureStyle.fontSize = 50;
        treasureStyle.normal.textColor = Color.yellow;

        levelStyle.fontSize = 30;
        levelStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 40), "Lives: " + gm.lives.ToString(), livesStyle);
        GUI.Label(new Rect(10, 60, 500, 40), "Boosts: " + this.GetComponent<PlayerCollider>().Boosts.ToString(), powerStyle);
        GUI.Label(new Rect(10, 110, 500, 40), "Treasures: " + gm.treasureFound.ToString(), treasureStyle);
        GUI.Label(new Rect(-40 + Screen.width / 2, 10, 300, 40), "Level: " + gm.level.ToString(), levelStyle);
    }
}
