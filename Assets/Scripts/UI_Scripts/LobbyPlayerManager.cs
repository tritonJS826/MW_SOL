using System.Collections.Generic;
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
            ReactEventHandler.OnPlayerJoinedAction += OnPlayerJoined;
        }
        
        private void OnPlayerJoined(string name, string uuid)
        {
            Color testColor = Color.HSVToRGB(Random.value, Random.value, 1f);   
            AddPlayer(uuid,testColor);
        }
    
        public void AddPlayer(string userUuid, Color color)
        {
            GameObject playerInfoObject = Instantiate(uiPlayerInfoPrefab, transform, false);
            float height = playerInfoObject.GetComponent<RectTransform>().sizeDelta.y;
            float yPos = -height * _playerInfoUIs.Count - offset * _playerInfoUIs.Count;
            playerInfoObject.transform.localPosition = startPosition + new Vector2(0,yPos);
            LobbyPlayerInfoUI playerInfoUI = playerInfoObject.GetComponent<LobbyPlayerInfoUI>();
            if (playerInfoUI != null)
            {
                int id = _playerInfoUIs.Count + 1;
                playerInfoUI.Initialize(id, userUuid, color, LobbyPlayerInfoUI.PlayerStatus.waiting);
            }
            _playerInfoUIs.Add(playerInfoUI);
        }
    }
}
