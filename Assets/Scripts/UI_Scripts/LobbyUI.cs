using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI_Scripts
{
    public class LobbyUI : MonoBehaviour
    {
        public void OnPressButtonStartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
