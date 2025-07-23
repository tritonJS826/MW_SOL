using System;
using System.Runtime.InteropServices;
using Data;
using UI_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReactEventHandler: MonoBehaviour
{
    [SerializeField] private TextAsset testJson;
    
    public static Action<QuestionList> OnQuestionListSetUpAction;
    public static Action<UserAnswerHandledByServer> OnServerSentAnswer;
    
    public static Action<string, string> OnPlayerJoinedAction;
    public static Action<SessionStateUpdated> OnSessionStateUpdatedAction;
    public static Action<string> OnUserReadyToPlay;
    public static Action<UserCapturedTarget> OnUserCapturedTargetAction;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DebugLog.Instance.AddText("Started ");
        
        
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GameStarted();
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
    public static extern void HostStartedGame();

    [DllImport("__Internal")]
    public static extern void GameFinished();
    
    [DllImport("__Internal")]
    public static extern void UserAnsweredQuestion(string questionUuid, string userAnswer);

    [DllImport("__Internal")]
    public static extern void UserCapturedTarget(string questionUuid);
    
    [DllImport("__Internal")]
    public static extern void UserReadyToStartPlay(string userUuid);
    
    
    
    
    // React -> Unity communication
    public void HandleSessionStateUpdated(string json)
    {
        DebugLog.Instance.AddText($"Received session state updated: {json}");
        var sessionState = JsonUtility.FromJson<SessionStateUpdated>(json);
        OnSessionStateUpdatedAction?.Invoke(sessionState);
        
        foreach (var user in sessionState.currentUsers)
        {
            if (user.userUuid == sessionState.selfUserUuid)
            {
                Game.playerId = user.userUuid;
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
        DebugLog.Instance.AddText($"Received user joined session: {json}");
        var user = JsonUtility.FromJson<UserJoinedSession>(json);
        OnPlayerJoinedAction?.Invoke(user.userUuid, user.userUuid);
    }
    
    public void HandleUserReadyToStartPlay(string json) // lobby
    {
        DebugLog.Instance.AddText($"Received user ready to start play: {json}");
        var user = JsonUtility.FromJson<UserReadyToStartPlay>(json);
        OnUserReadyToPlay?.Invoke(user.userUuid);
    }
    
    public void HandleHostStartedGame(string json) // lobby
    {
        DebugLog.Instance.AddText($"Received host started game: {json}");
        SceneManager.LoadScene(1);
        // var host = JsonUtility.FromJson<HostStartedGame>(json);
        // gameLogic.HostStartedGame(host);
    }
    
    public void HandleUserCapturedTarget(string json)
    {
        DebugLog.Instance.AddText($"Received user captured target: {json}"); 
        var userCapturedTarget = JsonUtility.FromJson<UserCapturedTarget>(json);
        OnUserCapturedTargetAction?.Invoke(userCapturedTarget);
    }
    
    public void HandleUserAnsweredQuestion(string json)
    {
        DebugLog.Instance.AddText($"User answered question: {json} ");
       
    }
    
    
    public void HandleUserAnswerHandledByServer(string json)
    {
        DebugLog.Instance.AddText($"Received user answer from server: {json}");
        var userAnswer = JsonUtility.FromJson<UserAnswerHandledByServer>(json);
        OnServerSentAnswer?.Invoke(userAnswer);
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