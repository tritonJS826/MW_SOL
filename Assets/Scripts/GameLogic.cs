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
    [SerializeField] private GameObject playerPrefab;
    
    public static Action<QuestionData> OnQuestionSelectedAction;
    
    private int _currentQuestionIndex = 0;
    private float _timer = 0f;
    private float _spawnInterval = 3f;

    private List<QuestionGameObject> _activeQuestions = new();
    private QuestionGameObject _currentSelectedQuestionByPlayer = null;

    private Dictionary<string, PlayerInfo> _players = new();
    
    private void Start()
    {
        PlayerInput.OnNextQuestionAction += OnNextQuestion;
        PlayerInput.OnQuestionClickedAction += UpdateCurrentSelectedQuestion;
        // we will create user with real uuid in HandleSessionStateUpdated
        // CreatePlayer("You", "SelfUuid");
    }


    public void CreatePlayer(string name, string uuid)
    {
        if (_players.ContainsKey(uuid))
        {
            UI.Instance.ShowDebugText($"Player with uuid {uuid} already exists.");
            return;
        }
        GameObject playerGO = Instantiate(playerPrefab, transform);
        PlayerInfo playerInfo = playerGO.GetComponent<PlayerInfo>();
        playerInfo.Initialize(name, Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));
        _players.Add(uuid, playerInfo);
        UI.Instance.ShowDebugText($"Player created: {name}");

        int offset = 0;
        int i = 0;
        foreach (var pInfo in _players.Values)
        {
            Vector3 position = Vector3.zero;
            position.y = -4.5f;

            int multiplier = i % 2 == 0 ? 1 : -1;
            position.x = offset * 2 * multiplier - 2;
            if (i % 2 == 0)
            {
                offset++;
            }
            pInfo.transform.localPosition = position;
            i++;
        }
    }
    
    public void OnSubmitAnswerButtonClicked(string answer)
    {
        if (_currentSelectedQuestionByPlayer == null)
        {
            return; // No question selected
        }
        
        ReactEventHandler.UserAnsweredQuestion(_currentSelectedQuestionByPlayer.QuestionData.uuid, answer);
        
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

    public void SetUpQuestions(QuestionList questions)
    {
        if (questions == null || questions.questions == null || questions.questions.Length == 0)
        {
           UI.Instance.ShowDebugText("No questions available to set up.");
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
        
        OnQuestionSelectedAction?.Invoke(_currentSelectedQuestionByPlayer != null ? _currentSelectedQuestionByPlayer.QuestionData : null);
    }


    private IEnumerator SpawnQuestions() 
    {
        while (_currentQuestionIndex < questionList.questions.Length)
        {
            yield return new WaitForSeconds(_spawnInterval);
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
        RemoveQuestionObjectFromTheList(questionGO);
        questionGO.StopAndDestroy(isCorrect);
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
        
        if(_activeQuestions.Count == 0)
        {
            ReactEventHandler.GameFinished();
        }
    }

    private void RemoveQuestionObjectFromTheList(QuestionGameObject questionGO)
    {
        if (questionGO == null)
        {
            Debug.LogError("QuestionGameObject is null in OnQuestionExpired");
            return;
        }
        
        if (_currentSelectedQuestionByPlayer == questionGO)
        {
            UpdateCurrentSelectedQuestion(null);
        }
        _activeQuestions.Remove(questionGO);
        
        if (_activeQuestions.Count > 0)
        {
            UpdateCurrentSelectedQuestion(_activeQuestions[0]);
        }
        else
        {
            OnQuestionSelectedAction?.Invoke(null);
        }
    }
}
