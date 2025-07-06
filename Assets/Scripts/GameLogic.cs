using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLogic: MonoBehaviour
{
    [SerializeField] private AllQuestionsSO allQuestions;
    [SerializeField] private QuestionGameObject questionPrefab;
    
    public static Action<QuestionSO> OnQuestionSelectedAction;
    
    private int _currentQuestionIndex = 0;
    private float _timer = 0f;
    private float _spawnInterval = 3f;

    private List<QuestionGameObject> _activeQuestions = new();
    private QuestionGameObject _currentSelectedQuestion = null;

    private void Start()
    {
        StartCoroutine(SpawnQuestions());
        PlayerInput.OnNextQuestionAction += OnNextQuestion;
        PlayerInput.OnQuestionClickedAction += UpdateCurrentSelectedQuestion;
    }
    
    private void OnNextQuestion()
    {
        if (_currentSelectedQuestion == null)
        {
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > 0 ? _activeQuestions[0] : null);
        }
        else
        {
            int index = _activeQuestions.IndexOf(_currentSelectedQuestion);
            index++;
            index %= _activeQuestions.Count;
            UpdateCurrentSelectedQuestion( _activeQuestions.Count > index ? _activeQuestions[index] : null);
        }
    }

    private void UpdateCurrentSelectedQuestion(QuestionGameObject questionGO)
    {
        _currentSelectedQuestion = questionGO;
        foreach (var q in _activeQuestions)
        {
            q.SetSelected(q == _currentSelectedQuestion);
        }
        
        OnQuestionSelectedAction?.Invoke(_currentSelectedQuestion != null ? _currentSelectedQuestion.QuestionData : null);
    }


    private IEnumerator SpawnQuestions() 
    {
        while (_currentQuestionIndex < allQuestions.GetQuestionCount())
        {
            yield return new WaitForSeconds(_spawnInterval);
            QuestionSO question = allQuestions.GetQuestion(_currentQuestionIndex);
            
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
    
    private void OnQuestionExpired()
    {
        if (_currentSelectedQuestion != null && _currentSelectedQuestion.QuestionData == _activeQuestions[0].QuestionData)
        {
            UpdateCurrentSelectedQuestion(null);
        }
        
        _activeQuestions.RemoveAt(0);
        
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
