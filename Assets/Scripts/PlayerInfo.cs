using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text playerNameText;
    
    
    public void Initialize(string playerName, Color color)
    {
        playerNameText.text = playerName;
        spriteRenderer.color = color;
    }
}