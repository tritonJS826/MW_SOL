using System.Runtime.InteropServices;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
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
        Test();
    }

    public void Test(string str)
    {
        timerText.color = Color.red;
        timerText.text = str;
    }
    
    public void JsonTest(string json)
    {
        
        
        timerText.color = Color.green;
        timerText.text = json;
    }


    
    [DllImport("__Internal")]
    private static extern void GameStarted ();
    
    [DllImport("__Internal")]
    private static extern void GameFinished ();

    public void Test()
    {
        GameStarted();
    }



    private void UpdateUI(QuestionData data)
    {
        if (data == null)
        {
            questionNameText.text = "";
            questionText.text = "";
            return;
        }
        
        questionNameText.text = data.Name;
        questionText.text = data.QuestionText;
        
        _timerTween?.Kill();
        _timerTween = null;
        
        float interval = 0.1f; 
        float totalDuration = data.TimeToAnswer;
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
