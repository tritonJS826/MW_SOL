using System;
using System.Runtime.InteropServices;
using Data;
using UnityEngine;

public class ReactEventHandler: MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private TextAsset testJson;
    
    
    public static Action<QuestionList> OnQuestionListSetUpAction;
    
    public static Action<string, string> OnPlayerJoinedAction;

    
    private void Start()
    {
        UI.Instance.ShowDebugText("Started ");
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GameStarted();
        UI.Instance.ShowDebugText("Started game in ReactEventHandler");
#endif
        
        GameLogic.OnQuestionSelectedAction += HandleQuestionSelected;
#if UNITY_EDITOR == true
        // For testing purposes, we can set up the question list directly
        
        HandleQuestionListReceived(testJson.text);
#endif
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
    
    
    // [ ] UserJoinedSession
    // [ ] UserReadyToStartPlay
    // [ ] HostStartedGame
    // [ ] UserCapturedTarget
    // [ ] UserAnsweredQuestion
    // [ ] UserAnswerHandledByServer
    
    // Server -> Unity communication

    public void HandleSessionStateUpdated(string json)
    {
        UI.Instance.ShowDebugText($"Received session state updated: {json}");
        var sessionState = JsonUtility.FromJson<SessionStateUpdated>(json);
        foreach (var user in sessionState.currentUsers)
        {
            if (user.userUuid == sessionState.selfUserUuid)
            {
                OnPlayerJoinedAction?.Invoke("You", user.userUuid);
            }
            else
            {
                OnPlayerJoinedAction?.Invoke(user.userUuid, user.userUuid);
            }
        }
    }

    public void HandleUserJoinedSession(string json)
    {
        UI.Instance.ShowDebugText($"Received user joined session: {json}");
        var user = JsonUtility.FromJson<UserJoinedSession>(json);
        OnPlayerJoinedAction?.Invoke(user.userUuid, user.userUuid);
    }
    
    public void HandleUserReadyToStartPlay(string json) // lobby
    {
        UI.Instance.ShowDebugText($"Received user ready to start play: {json}");
        // var user = JsonUtility.FromJson<UserReadyToStartPlay>(json);
        // gameLogic.UserReadyToStartPlay(user);
    }
    
    public void HandleHostStartedGame(string json) // lobby
    {
        UI.Instance.ShowDebugText($"Received host started game: {json}");
        // var host = JsonUtility.FromJson<HostStartedGame>(json);
        // gameLogic.HostStartedGame(host);
    }
    
    public void HandleUserCapturedTarget(string json)
    {
        UI.Instance.ShowDebugText($"Received user captured target: {json}");
        // var userCapturedTarget = JsonUtility.FromJson<UserCapturedTarget>(json);
        // gameLogic.UserCapturedTarget(userCapturedTarget);
    }
    
    public void HandleUserAnsweredQuestion(string json)
    {
        UI.Instance.ShowDebugText($"User answered question: {json} ");
        // UserCapturedTarget(questionUuid);
        // UserAnsweredQuestion(questionUuid, userAnswer);
    }
    
    
    public void HandleUserAnswerHandledByServer(string json)
    {
        UI.Instance.ShowDebugText($"Received user answer from server: {json}");
        var userAnswer = JsonUtility.FromJson<UserAnswerHandledByServer>(json);
        gameLogic.ServerSentQuestionAnswer(userAnswer);
    }
    
    public void HandleQuestionListReceived(string json)
    {
        var questionList = JsonUtility.FromJson<QuestionList>(json);
        foreach (var question in questionList.questions)
        {
            if (question.timeToAnswer <= 0)
            {
                question.timeToAnswer = 60f;
            }
        }
        OnQuestionListSetUpAction?.Invoke(questionList);
    }
    
    
    
    private void HandleQuestionSelected(QuestionData questionData, float remainingTime)
    {
        if (questionData == null) return;
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        UserCapturedTarget(questionData.uuid);
#endif
    }
    
    private void OnDestroy()
    {
        GameLogic.OnQuestionSelectedAction -= HandleQuestionSelected;
    }
  
}