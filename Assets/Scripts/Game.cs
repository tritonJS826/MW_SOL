using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public static readonly int TimeBetweenNextQuestion = 10;
    public static string playerId;
    public static Dictionary<string, Color> colorsForPlayers = new();
}