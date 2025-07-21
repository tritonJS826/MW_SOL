using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI: MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    [SerializeField] private TMP_Text globalTimerText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text questionNameText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private TMP_Text debugText;

    private Tween _timerTween;
    private Tween _globalTimerTween;
    
    // FIXME: only for testing purposes, remove later
    public static UI Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        answerInputField.onSubmit.AddListener((string answer) => OnSubmitAnswer());
        GameLogic.OnQuestionSelectedAction += UpdateUI;
        ReactEventHandler.OnQuestionListSetUpAction += SetUpGlobalTimer;
    }
    
    public void Start()
    {
        UpdateUI(null, 0);
    }

    public void ShowDebugText(string text)
    {
        debugText.text = debugText.text + "\n\n" + text;
    }
    
    private void SetUpGlobalTimer(QuestionList questionList)
    {
        if (questionList == null || questionList.questions.Length == 0)
        {
            timerText.text = "00:00";
            return;
        }

        float totalTime = Game.TimeBetweenNextQuestion;
        var maxRemainingQuestionTime = 0f;
        
        foreach (var question in questionList.questions)
        {
            maxRemainingQuestionTime -= Game.TimeBetweenNextQuestion;
            maxRemainingQuestionTime = Mathf.Max(maxRemainingQuestionTime, 0);
            
            if (question.timeToAnswer > maxRemainingQuestionTime)
            {
                maxRemainingQuestionTime = question.timeToAnswer;
            }
            totalTime += Game.TimeBetweenNextQuestion;
        }
        totalTime += maxRemainingQuestionTime;
        
        int loops = Mathf.FloorToInt(totalTime / 0.3f);
        _globalTimerTween?.Kill();
        _globalTimerTween = DOVirtual.DelayedCall(0.3f, () =>
        {
            globalTimerText.text = $"{totalTime:F0}";
            totalTime -= 0.3f;
        }).SetLoops(loops, LoopType.Restart);
    }
    
    private void UpdateUI(QuestionData data, float remainingTime)
    {
        if (data == null)
        {
            questionNameText.text = "";
            questionText.text = "";
            timerText.text = "";
            return;
        }
        
        questionNameText.text = data.name;
        questionText.text = data.questionText;
        
        _timerTween?.Kill();
        _timerTween = null;
        
        float interval = 0.1f;
        float totalDuration = remainingTime;
        int loops = Mathf.FloorToInt(totalDuration / interval);
        
        _timerTween = DOVirtual.DelayedCall(interval, () =>
        {
            totalDuration -= interval;
            timerText.text = $"{totalDuration:F1} ";
        }).SetLoops(loops, LoopType.Restart);
        
        answerInputField.ActivateInputField();
    }
    
    public void OnSubmitAnswer()
    {
        string answer = answerInputField.text.Trim();
        gameLogic.OnSubmitAnswerButtonClicked(answer);
        answerInputField.text = "";
        
        Debug.Log("Submitted answer: " + answer);
    }
}
