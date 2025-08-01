using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
            ReactEventHandler.OnUserReadyToPlay += OnUserReady;
        }

        private void OnUserReady(string userUuid)
        {
            foreach (var user in _playerInfoUIs)
            {
                if (user.GetUuid() == userUuid)
                {
                    user.ChangeStatus(LobbyPlayerInfoUI.PlayerStatus.ready);
                    return;
                }
            }
        }
        
        private void OnPlayerJoined(string name, string uuid)
        {
            AddPlayer(name, uuid);
        }
    
        private void AddPlayer(string userName, string userUuid)
        {
            foreach (var pl in _playerInfoUIs)
            {
                if (pl.GetUuid() == userUuid)
                {
                    return;
                }
            }

            Color color = new Color(Random.value, Random.value, Random.value, 1);
            Game.colorsForPlayers[userUuid] = color;
            
            GameObject playerInfoObject = Instantiate(uiPlayerInfoPrefab, transform, false);
            float height = playerInfoObject.GetComponent<RectTransform>().sizeDelta.y;
            float yPos = -height * _playerInfoUIs.Count - offset * _playerInfoUIs.Count;
            playerInfoObject.transform.localPosition = startPosition + new Vector2(0,yPos);
            LobbyPlayerInfoUI playerInfoUI = playerInfoObject.GetComponent<LobbyPlayerInfoUI>();
            if (playerInfoUI != null)
            {
                int id = _playerInfoUIs.Count + 1;
                // TODO: if you will replace userUuid on userName here - duplicate of playerInfoUI appears in the lobby
                playerInfoUI.Initialize(id, userUuid, color, LobbyPlayerInfoUI.PlayerStatus.waiting);
            }
            _playerInfoUIs.Add(playerInfoUI);
        }


        private void OnDestroy()
        {
            ReactEventHandler.OnPlayerJoinedAction -= OnPlayerJoined;
            ReactEventHandler.OnUserReadyToPlay -= OnUserReady;
        }
    }
}
