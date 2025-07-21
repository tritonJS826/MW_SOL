using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    
    
    public void OnPressButtonStartGame()
    {
        SceneManager.LoadScene(1);
    }
}
