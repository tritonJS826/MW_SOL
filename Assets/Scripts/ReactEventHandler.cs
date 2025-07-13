using System.Runtime.InteropServices;
using Data;
using UnityEngine;

public class ReactEventHandler: MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    private void Awake()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    GameStarted();
#endif
        
        GameLogic.OnQuestionSelectedAction += HandleQuestionSelected;
    }

    public void HandleQuestionListReceived(string json)
    {
        var questionList = JsonUtility.FromJson<QuestionList>(json);
        gameLogic.SetUpQuestions(questionList);
    }

    // Unity -> server communication
    [DllImport("__Internal")]
    private static extern void GameStarted();

    [DllImport("__Internal")]
    public static extern void GameFinished();
    
    [DllImport("__Internal")]
    public static extern void UserAnsweredQuestion(string questionUuid, string userAnswer);

    [DllImport("__Internal")]
    public static extern void UserCapturedTarget(string questionUuid);

    
    // Server -> Unity communication
    public void HandleUserAnswerHandledByServer(string json)
    {
        var userAnswer = JsonUtility.FromJson<UserAnswerHandledByServer>(json);
        gameLogic.ServerSentQuestionAnswer(userAnswer);
    }
    
    
    // 
    
    private void HandleQuestionSelected(QuestionData questionData)
    {
        if (questionData == null) return;
        
        UserCapturedTarget(questionData.uuid);
    }
    
    private void OnDestroy()
    {
        GameLogic.OnQuestionSelectedAction -= HandleQuestionSelected;
    }
  
}