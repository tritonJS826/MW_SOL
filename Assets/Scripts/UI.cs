using System.Runtime.InteropServices;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI: MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text questionNameText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private TMP_InputField answerInputField;

    private Tween _timerTween;
    
    public void Start()
    {
        GameLogic.OnQuestionSelectedAction += UpdateUI;
        UpdateUI(null);
    }

    
    private void UpdateUI(QuestionData data)
    {
        if (data == null)
        {
            questionNameText.text = "";
            questionText.text = "";
            return;
        }
        
        questionNameText.text = data.name;
        questionText.text = data.questionText;
        
        _timerTween?.Kill();
        _timerTween = null;
        
        float interval = 0.1f; 
        float totalDuration = data.timeToAnswer;
        int loops = Mathf.FloorToInt(totalDuration / interval);
        _timerTween = DOVirtual.DelayedCall(interval, () =>
        {
            timerText.text = $"{totalDuration:F1} seconds remaining";
        }).SetLoops(loops, LoopType.Restart);
    }
    
    public void OnSubmitAnswer()
    {
        string answer = answerInputField.text.Trim();
        gameLogic.OnSubmitAnswerButtonClicked(answer);
    }
}
