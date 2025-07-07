using System.Runtime.InteropServices;
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
        SomeMethod();

    }

    public void Test(string str)
    {
        timerText.color = Color.red;
        timerText.text = str;
    }
    
    public void JsonTest(string json)
    {
        json = JsonUtility.FromJson(json, typeof(string)) as string;
        if (string.IsNullOrEmpty(json))
        {
            timerText.color = Color.red;
            timerText.text = "Invalid JSON";
            return;
        }
        
        timerText.color = Color.green;
        timerText.text = json;
    }


    
    [DllImport("__Internal")]
    private static extern void GameStart (string userName, int score);

    public void SomeMethod()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    GameStart ("Player1", 100);
#endif
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
    
    public void OnSubmitAnswer()
    {
        string answer = answerInputField.text.Trim();
        gameLogic.OnSubmitAnswerButtonClicked(answer);
    }
    
    
}
