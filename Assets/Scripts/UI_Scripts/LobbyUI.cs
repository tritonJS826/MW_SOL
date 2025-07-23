using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using TMPro;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class LobbyUI: MonoBehaviour
    { 
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject readyButton;
        [SerializeField] private TMP_Text sessionIdText;
        
        public void OnPressButtonStartGame()
        {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            ReactEventHandler.HostStartedGame();
            return;
#endif
            Game.playerId = "Player";
            Game.colorsForPlayers.Add(Game.playerId, Color.white);
            SceneManager.LoadScene(1);
        }
        
        private void Start()
        {
            ReactEventHandler.OnSessionStateUpdatedAction += OnSessionStateUpdate;
        }

        private void OnSessionStateUpdate(SessionStateUpdated state)
        {
            sessionIdText.text = state.shareUrl;
            startButton.SetActive(state.selfUserUuid == state.userHostUuid);
            readyButton.SetActive(state.selfUserUuid != state.userHostUuid);
        }

        public void OnButtonReadyClicked()
        {
            ReactEventHandler.UserReadyToStartPlay(Game.playerId);
            readyButton.GetComponent<Button>().interactable = false;
        }
        
        
        public void OnFullScreeButtonSwitchValue()
        {
            Screen.fullScreen = !Screen.fullScreen;
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        private void OnDestroy()
        {
            ReactEventHandler.OnSessionStateUpdatedAction -= OnSessionStateUpdate;
        }
    }
}
