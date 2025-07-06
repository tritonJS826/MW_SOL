using UnityEngine;

[CreateAssetMenu(fileName = "AllQuestions", menuName = "ScriptableObject/AllQuestions")]
public class AllQuestionsSO : ScriptableObject
{
    [SerializeField] private QuestionSO[] questions;

    public QuestionSO[] Questions => questions;

    public int GetQuestionCount()
    {
        return questions.Length;
    }

    public QuestionSO GetQuestion(int index)
    {
        if (index < 0 || index >= questions.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return null;
        }
        return questions[index];
    }
        
}