using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class LobbyPlayerInfoUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text idText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Image playerColor;

        [SerializeField] private Color readyColor;
        [SerializeField] private Color waitingColor;

        private string userUuid;
        public enum PlayerStatus
        {
            ready,
            waiting
        }
        
        public void Initialize(int num, string playerId, Color color, PlayerStatus status)
        {
            userUuid = playerId;
            idText.text = num.ToString();
            playerNameText.text = playerId;
            statusText.text = status.ToString();
            ChangeStatusTextColor(status);
            playerColor.color = color;
        }

        public void ChangeStatus(PlayerStatus status)
        {
            statusText.text = status.ToString();
            ChangeStatusTextColor(status);
        }

        private void ChangeStatusTextColor(PlayerStatus status)
        {
            statusText.color = status == PlayerStatus.ready ? readyColor: waitingColor;
        }

        public string GetUuid()
        {
            return userUuid;
        }
        
    }
}

