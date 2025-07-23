using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UI_Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLogic: MonoBehaviour
{
    [SerializeField] private QuestionList questionList;
    [SerializeField] private QuestionGameObject questionPrefab;
    
    public static Action<QuestionData,float> OnQuestionSelectedAction;

    private int _currentQuestionIndex;

    private readonly List<QuestionGameObject> _activeQuestions = new();
    private Dictionary<string, QuestionGameObject> _selectedQuestions = new();


    private void Start()
    {
        PlayerInput.OnNextQuestionAction += OnNextQuestion;
        PlayerInput.OnQuestionClickedAction += UpdateCurrentSelectedQuestion;
        ReactEventHandler.OnQuestionListSetUpAction += SetUpAllQuestions;
        ReactEventHandler.OnServerSentAnswer += ServerSentQuestionAnswer;
        ReactEventHandler.OnUserCapturedTargetAction += OnSomePlayerSelectedTarget;
    }



    public void OnSubmitAnswerButtonClicked(string answer, string playerUuid)
    {
        if (_selectedQuestions.ContainsKey(Game.playerId))
        {
            return;
        }
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        ReactEventHandler.UserAnsweredQuestion(_selectedQuestions[Game.playerId].QuestionData.uuid, answer);
#endif
        _selectedQuestions[Game.playerId].SetWaitingForAnswer(true);
        
        OnNextQuestion();
    }

    private void OnSomePlayerSelectedTarget(UserCapturedTarget target)
    {
        foreach (var gm in _activeQuestions)
        {
            if (gm.GetQuestionData().uuid == target.questionUuid)
            {
                DebugLog.Instance.AddText("MakeUnselected " + target.userUuid + "  " + gm.GetQuestionData().uuid );
                _selectedQuestions[target.userUuid].SetSelected(false);
                _selectedQuestions[target.userUuid] = gm;
                gm.SetSelected(true, Game.colorsForPlayers[target.userUuid]);
            }
        }
    }

    private void ServerSentQuestionAnswer(UserAnswerHandledByServer userAnswer)
    {
        DebugLog.Instance.AddText($"isOk: {userAnswer.isOk}, questionUuid: {userAnswer.questionUuid}, answer: {userAnswer.userAnswer} playerUuid: {userAnswer.userUuid}");
        DebugLog.Instance.AddText($"Server sent answer for question {userAnswer.questionUuid}: {userAnswer.isOk}");
        foreach (var questionGO in _activeQuestions)
        {
            if (questionGO.QuestionData.uuid == userAnswer.questionUuid)
            {
                DebugLog.Instance.AddText($"I found my gm: {questionGO.QuestionData.name} with answer: {userAnswer.isOk}");
                OnQuestionAnswered(questionGO, userAnswer.isOk);
                return;
            }
        }
    }

    private void SetUpAllQuestions(QuestionList questions)
    {
        if (questions == null || questions.questions == null || questions.questions.Length == 0)
        {
            questionList = new QuestionList { questions = Array.Empty<QuestionData>() };
            _currentQuestionIndex = 0;
            DebugLog.Instance.AddText("No questions available to set up.");
            CheckForGameEnd();
            return;
        }
        
        questionList = questions;
        _currentQuestionIndex = 0;
        
        foreach (var questionGO in _activeQuestions)
        {
            if (questionGO != null)
            {
                questionGO.StopAllTwens();
                Destroy(questionGO.gameObject);
            }
        }
        _activeQuestions.Clear();
        
        StopAllCoroutines();
        StartCoroutine(SpawnQuestions());
    }

    private bool CheckForGameEnd(bool callGameFinishedEvent = true)       
    {                                                                     
        // Game ends if:                                                  
        // 1. All questions from the questionList have been spawned (_currentQuestionIndex has reached or exceeded list length)                 
        // AND                                                            
        // 2. There are no active questions left on screen (_activeQuestions list is empty)                                            

        bool allQuestionsSpawned = (questionList == null)
            || (questionList.questions == null)
            || (_currentQuestionIndex >= questionList.questions.Length);
        bool noActiveQuestions = _activeQuestions.Count == 0;
               
        if (allQuestionsSpawned && noActiveQuestions)                     
        {                                                                 
            DebugLog.Instance.AddText("All questions processed (spawned and cleared). Game Finished!");                                             
            if (callGameFinishedEvent)
            {
                ReactEventHandler.GameFinished();
            }
            StopAllCoroutines(); 
            return true;
        }
        return false;
    }
    
    private void OnNextQuestion()
    {
        if (!_selectedQuestions.ContainsKey(Game.playerId))
        {
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > 0 ? _activeQuestions[0] : null);
        }
        else
        {
            int index = _activeQuestions.FindIndex(data => data.GetQuestionData().uuid ==
                                                           _selectedQuestions[Game.playerId].GetQuestionData().uuid);
            index++;
            index %= _activeQuestions.Count;
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > index ? _activeQuestions[index] : null);
        }
    }

    private void UpdateCurrentSelectedQuestion(QuestionGameObject questionGO)
    {
        if (_selectedQuestions.ContainsKey(Game.playerId))
        {
            _selectedQuestions[Game.playerId].SetSelected(false);
        }
        _selectedQuestions[Game.playerId] = questionGO;
        Color color = Game.colorsForPlayers[Game.playerId];
        _selectedQuestions[Game.playerId].SetSelected(true, color);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        ReactEventHandler.UserCapturedTarget(questionGO.GetQuestionData().uuid);
         DebugLog.Instance.AddText("I SENDIN_EVENT  " + questionGO.GetQuestionData().uuid + "  " + Game.playerId );
#endif
        
        if (questionGO == null)
        {
            OnQuestionSelectedAction?.Invoke(null, 0);
            return;
        }
        
        OnQuestionSelectedAction?.Invoke(questionGO.QuestionData, questionGO.GetRemainingTime());
    }


    private IEnumerator SpawnQuestions() 
    {
        while (_currentQuestionIndex < questionList.questions.Length)
        {
            yield return new WaitForSeconds(Game.TimeBetweenNextQuestion);

            if (CheckForGameEnd(false))
            {
                yield break;
            }
            QuestionData question = questionList.questions[_currentQuestionIndex];
            
            if (question != null)
            {
                QuestionGameObject questionGO = Instantiate(questionPrefab, transform.parent);
                questionGO.Initialize(question, OnQuestionExpired);
                questionGO.transform.position = new Vector3(Random.Range(-5,5), 5, 0); // Set position as needed
                questionGO.StartMoving(new Vector2(0,-4.5f)); // Move to a target position
                _activeQuestions.Add(questionGO);
                _currentQuestionIndex++;
            }
            else
            {
                Debug.LogError("Question not found at index: " + _currentQuestionIndex);
            }
        }
    }
    
    private void OnQuestionAnswered(QuestionGameObject questionGO, bool isCorrect)
    {
        Debug.Log($"Question answered: {questionGO.QuestionData.name}, Correct: {isCorrect}");
        RemoveQuestionObjectFromTheList(questionGO, true);
        questionGO.StopAndDestroy(isCorrect);
        CheckForGameEnd();
    }
    
    private void OnQuestionExpired(QuestionGameObject questionGO)
    {
        if (questionGO == null)
        {
            return;
        }
        
        RemoveQuestionObjectFromTheList(questionGO);
        questionGO.StopAllTwens();
        Destroy(questionGO.gameObject);
        
 
        CheckForGameEnd();
    }

    private void RemoveQuestionObjectFromTheList(QuestionGameObject questionGO, bool byPlayerAction = false)
    {
        if (questionGO == null)
        {
            Debug.LogError("QuestionGameObject is null in OnQuestionExpired");
            return;
        }

        bool wasSelectedByPlayer = questionGO == _selectedQuestions[Game.playerId];

        _activeQuestions.Remove(questionGO);

        if (byPlayerAction || wasSelectedByPlayer)
        {                                                                     
            QuestionGameObject nextSelectedQuestion = null;                   
                                                                              
            if (_activeQuestions.Count > 0)                                   
            {                                                                 
                // If the currently selected question (before removal) is still valid and present,                                                    
                // and it wasn't the one that just got removed (i.e., a non-selected question expired),                                                 
                // then keep it selected to maintain focus.                   
                if (!wasSelectedByPlayer && _selectedQuestions[Game.playerId] != null && _activeQuestions.Contains(_selectedQuestions[Game.playerId]))             
                {                                                             
                    nextSelectedQuestion = _selectedQuestions[Game.playerId];  
                }                                                             
                else // Otherwise, select the first available question in the list    
                {                                                             
                    nextSelectedQuestion = _activeQuestions[0];               
                }                                                             
            }                                                                 
            // Update the current selected question. If nextSelectedQuestion is null, it effectively deselects everything.                               
            UpdateCurrentSelectedQuestion(nextSelectedQuestion);              
        }
    }
    
    private void OnDestroy()
    {
        PlayerInput.OnNextQuestionAction -= OnNextQuestion;
        PlayerInput.OnQuestionClickedAction -= UpdateCurrentSelectedQuestion;
        ReactEventHandler.OnQuestionListSetUpAction -= SetUpAllQuestions;
        ReactEventHandler.OnServerSentAnswer -= ServerSentQuestionAnswer;
    }
}
