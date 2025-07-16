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
    [SerializeField] private TMP_Text debugText;

    private Tween _timerTween;
    
    // FIXME: only for testing purposes, remove later
    public static UI Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        answerInputField.onSubmit.AddListener((string answer) => OnSubmitAnswer());
    }
    
    public void Start()
    {
        GameLogic.OnQuestionSelectedAction += UpdateUI;
        UpdateUI(null);
    }

    public void ShowDebugText(string text)
    {
        debugText.text = debugText.text + "\n\n" + text;
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
