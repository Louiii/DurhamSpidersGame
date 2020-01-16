using UnityEngine;

public class NotHiding : MonoBehaviour
{
    private GUIStyle hidingStyle = new GUIStyle();

    private string hidingStatus = "Press h to hide";
    private int timeLeft;

    void Start()
    {
        hidingStyle.fontSize = 40;
        hidingStyle.normal.textColor = Color.white;
    }

    public void StartTimer(int t)
    {
        timeLeft = t;
        DecrementTimer();
    }

    void DecrementTimer()
    {
        if (timeLeft > 0)
        {
            timeLeft -= 1;
            hidingStatus = "Wait " + timeLeft.ToString() + "s";
            Invoke("DecrementTimer", 1.0f);
        }
        else
        {
            hidingStatus = "Press h to hide";
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300, 10, 300, 40), hidingStatus, hidingStyle);
    }
}
