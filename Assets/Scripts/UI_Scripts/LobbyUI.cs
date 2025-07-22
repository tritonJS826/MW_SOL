using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

namespace UI_Scripts
{
    public class LobbyUI : MonoBehaviour
    {

        [DllImport("__Internal")]
        public static extern void HostStartedGame();

        public void OnPressButtonStartGame()
        {
            HostStartedGame();
        }
    }
}
