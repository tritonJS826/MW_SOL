using Data;
using UnityEngine;

public class ReactEventHandler : MonoBehaviour
{
    public void HandleQuestionListReceived(string json)
    {
        var questionList = JsonUtility.FromJson<QuestionListReceived>(json);
        Debug.LogError($"Question List Received: {questionList.Name}, Number: {questionList.Number}, Question: {questionList.QuestionText}, UUID: {questionList.Uuid}, Time to Answer: {questionList.TimeToAnswer}");
    }

    public void UserAnswerHandledByServer(string json)
    {
        var userAnswer = JsonUtility.FromJson<UserAnswerHandledByServer>(json);
        Debug.LogError($"User Answer Handled: IsOk: {userAnswer.IsOk}, UserUuid: {userAnswer.UserUuid}, UserAnswer: {userAnswer.UserAnswer}, QuestionName: {userAnswer.QuestionName}, QuestionDescription: {userAnswer.QuestionDescription}, QuestionAnswer: {userAnswer.QuestionAnswer}, ResultDescription: {userAnswer.ResultDescription}, QuestionUuid: {userAnswer.QuestionUuid}, Uuid: {userAnswer.Uuid}");
    }
}