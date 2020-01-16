using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public Texture2D crosshairImage;

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");
    }
}
