using Data;
using UnityEngine;

public class ReactEventHandler : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    public void HandleQuestionListReceived(string json)
    {
        var questionList = JsonUtility.FromJson<QuestionList>(json);
        Debug.LogError($"Question List Received: {questionList.Questions.Length} questions");
        foreach (var question in questionList.Questions)
        {
            Debug.LogError($"Answer: {question.Answer}, Name: {question.Name}, Order: {question.Order}, QuestionText: {question.QuestionText}, Uuid: {question.Uuid}, TimeToAnswer: {question.TimeToAnswer}");
        }
    }

    public void UserAnswerHandledByServer(string json)
    {
        var userAnswer = JsonUtility.FromJson<UserAnswerHandledByServer>(json);
        Debug.LogError($"User Answer Handled: IsOk: {userAnswer.IsOk}, UserUuid: {userAnswer.UserUuid}, UserAnswer: {userAnswer.UserAnswer}, QuestionName: {userAnswer.QuestionName}, QuestionDescription: {userAnswer.QuestionDescription}, QuestionAnswer: {userAnswer.QuestionAnswer}, ResultDescription: {userAnswer.ResultDescription}, QuestionUuid: {userAnswer.QuestionUuid}, Uuid: {userAnswer.Uuid}");
    }
}