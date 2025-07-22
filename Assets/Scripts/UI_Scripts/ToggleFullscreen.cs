using UnityEngine;

public class ToggleFullscreen : MonoBehaviour
{
    public void Change()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
