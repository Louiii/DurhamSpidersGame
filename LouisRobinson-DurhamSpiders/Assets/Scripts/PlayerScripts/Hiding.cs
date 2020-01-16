using UnityEngine;

public class Hiding : MonoBehaviour
{
    private GUIStyle hidingStyle = new GUIStyle();
    private int timeLeft = 10;

    void Start()
    {
        hidingStyle.fontSize = 50;
        hidingStyle.normal.textColor = Color.green;
    }

    public void StartTimer()
    {
        timeLeft = 10;
        DecrementTimer();
    }

    void DecrementTimer()
    {
        if (timeLeft > 0)
        {
            timeLeft -= 1;
            Invoke("DecrementTimer", 1.0f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300, 10, 300, 40), "Hiding, "+ timeLeft.ToString()+"s", hidingStyle);
    }
}
