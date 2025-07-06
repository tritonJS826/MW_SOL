using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text questionNameText;
    [SerializeField] private TMP_Text questionText;

    private Tween _timerTween;
    public void Start()
    {
        GameLogic.OnQuestionSelectedAction += UpdateUI;
        
        UpdateUI(null); 
    }
    
    private void UpdateUI(QuestionSO question)
    {
        if (question == null)
        {
            questionNameText.text = "";
            questionText.text = "";
            return;
        }
        
        questionNameText.text = question.QuestionName;
        questionText.text = question.QuestionText;
        
        _timerTween?.Kill();
        _timerTween = null;
        
        float interval = 0.1f; 
        float totalDuration = question.SpeedTime;
        int loops = Mathf.FloorToInt(totalDuration / interval);
        _timerTween = DOVirtual.DelayedCall(interval, () =>
        {
            timerText.text = $"{totalDuration:F1} seconds remaining";
        }).SetLoops(loops, LoopType.Restart);
    }
    
    
    
}
