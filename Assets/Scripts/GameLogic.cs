using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLogic: MonoBehaviour
{
    [SerializeField] private QuestionList questionList;
    [SerializeField] private QuestionGameObject questionPrefab;
    
    public static Action<QuestionData,float> OnQuestionSelectedAction;

    private int _currentQuestionIndex;

    private readonly List<QuestionGameObject> _activeQuestions = new();
    private QuestionGameObject _currentSelectedQuestionByPlayer = null;


    private void Start()
    {
        PlayerInput.OnNextQuestionAction += OnNextQuestion;
        PlayerInput.OnQuestionClickedAction += UpdateCurrentSelectedQuestion;
        ReactEventHandler.OnQuestionListSetUpAction += SetUpAllQuestions;
    }


   
    public void OnSubmitAnswerButtonClicked(string answer)
    {
        if (_currentSelectedQuestionByPlayer == null)
        {
            return;
        }
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        ReactEventHandler.UserAnsweredQuestion(_currentSelectedQuestionByPlayer.QuestionData.uuid, answer);
#endif
        _currentSelectedQuestionByPlayer.SetWaitingForAnswer(true);
        
        OnNextQuestion();
    }

    public void ServerSentQuestionAnswer(UserAnswerHandledByServer userAnswer)
    {
        UI.Instance.ShowDebugText($"isOk: {userAnswer.isOk}, questionUuid: {userAnswer.questionUuid}, answer: {userAnswer.userAnswer} playerUuid: {userAnswer.userUuid}");
        UI.Instance.ShowDebugText($"Server sent answer for question {userAnswer.questionUuid}: {userAnswer.isOk}");
        foreach (var questionGO in _activeQuestions)
        {
            if (questionGO.QuestionData.uuid == userAnswer.questionUuid)
            {
                UI.Instance.ShowDebugText($"I found my gm: {questionGO.QuestionData.name} with answer: {userAnswer.isOk}");
                OnQuestionAnswered(questionGO, userAnswer.isOk);
                return;
            }
        }
    }

    public void SetUpAllQuestions(QuestionList questions)
    {
        if (questions == null || questions.questions == null || questions.questions.Length == 0)
        {
            questionList = new QuestionList { questions = new QuestionData[0] };
            _currentQuestionIndex = 0;
            UI.Instance.ShowDebugText("No questions available to set up.");
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
            UI.Instance.ShowDebugText("All questions processed (spawned and cleared). Game Finished!");                                             
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
        if (_currentSelectedQuestionByPlayer == null)
        {
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > 0 ? _activeQuestions[0] : null);
        }
        else
        {
            int index = _activeQuestions.IndexOf(_currentSelectedQuestionByPlayer);
            index++;
            index %= _activeQuestions.Count;
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > index ? _activeQuestions[index] : null);
        }
    }

    private void UpdateCurrentSelectedQuestion(QuestionGameObject questionGO)
    {
        _currentSelectedQuestionByPlayer = questionGO;
        foreach (var q in _activeQuestions)
        {
            q.SetSelected(q == _currentSelectedQuestionByPlayer);
        }
        
        // TODO : importonant
        
        if (_currentSelectedQuestionByPlayer == null)
        {
            OnQuestionSelectedAction?.Invoke(null, 0);
            return;
        }
        float remainingTime = _currentSelectedQuestionByPlayer.QuestionData.timeToAnswer;
        OnQuestionSelectedAction?.Invoke(_currentSelectedQuestionByPlayer.QuestionData, remainingTime);
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

        bool wasSelected = (_currentSelectedQuestionByPlayer == questionGO);

        _activeQuestions.Remove(questionGO);

        if (byPlayerAction || wasSelected)
        {                                                                     
            QuestionGameObject nextSelectedQuestion = null;                   
                                                                              
            if (_activeQuestions.Count > 0)                                   
            {                                                                 
                // If the currently selected question (before removal) is still valid and present,                                                    
                // and it wasn't the one that just got removed (i.e., a non-selected question expired),                                                 
                // then keep it selected to maintain focus.                   
                if (!wasSelected && _currentSelectedQuestionByPlayer != null && _activeQuestions.Contains(_currentSelectedQuestionByPlayer))             
                {                                                             
                    nextSelectedQuestion = _currentSelectedQuestionByPlayer;  
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
    }
}
