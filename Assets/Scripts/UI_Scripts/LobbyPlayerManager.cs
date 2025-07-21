using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UI_Scripts
{
    public class LobbyPlayerManager: MonoBehaviour
    {
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private float offset;
        [SerializeField] private GameObject uiPlayerInfoPrefab;
    
        private readonly List<LobbyPlayerInfoUI> _playerInfoUIs = new();
        
        private void Start()
        {
            UserInfo testUser = new UserInfo("TestUser");
            testUser.userUuid = "12345-67890";
            Color testColor = Color.red;
            AddPlayer(testUser, testColor);
            testUser = new UserInfo("AnotherUser");
            testUser.userUuid = "54321-09876";
            testColor = Color.blue;
            AddPlayer(testUser, testColor);
        }
    
        public void AddPlayer(UserInfo userInfo, Color color)
        {
            GameObject playerInfoObject = Instantiate(uiPlayerInfoPrefab, transform, false);
            float height = playerInfoObject.GetComponent<RectTransform>().sizeDelta.y;
            float yPos = -height * _playerInfoUIs.Count - offset * _playerInfoUIs.Count;
            playerInfoObject.transform.localPosition = startPosition + new Vector2(0,yPos);
            LobbyPlayerInfoUI playerInfoUI = playerInfoObject.GetComponent<LobbyPlayerInfoUI>();
            if (playerInfoUI != null)
            {
                int id = _playerInfoUIs.Count + 1;
                playerInfoUI.Initialize(id, userInfo.userUuid, color, LobbyPlayerInfoUI.PlayerStatus.waiting);
            }
            _playerInfoUIs.Add(playerInfoUI);
        }
    }
}
