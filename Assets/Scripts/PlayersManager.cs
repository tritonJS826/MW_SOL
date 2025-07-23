using System.Collections.Generic;
using UI_Scripts;
using UnityEngine;

public class PlayersManager: MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
        
    private readonly Dictionary<string, PlayerInfo> _players = new();


    private void Awake()
    {
        ReactEventHandler.OnPlayerJoinedAction += CreatePlayer;
    }

    private void CreatePlayer(string name, string userUuid)
    {
        if (_players.ContainsKey(userUuid))
        {
            DebugLog.Instance.AddText($"Player with uuid {userUuid} already exists.");
            return;
        }
        GameObject playerGO = Instantiate(playerPrefab, transform);
        PlayerInfo playerInfo = playerGO.GetComponent<PlayerInfo>();
        playerInfo.Initialize(name, Game.colorsForPlayers[userUuid]);
        _players.Add(userUuid, playerInfo);
        DebugLog.Instance.AddText($"Player created: {name}");

        int offset = 0;
        int i = 0;
        foreach (var pInfo in _players.Values)
        {
            Vector3 position = Vector3.zero;
            position.y = -4.5f;

            int multiplier = i % 2 == 0 ? 1 : -1;
            position.x = offset * 2 * multiplier - 2;
            if (i % 2 == 0)
            {
                offset++;
            }
            pInfo.transform.localPosition = position;
            i++;
        }
    }

}