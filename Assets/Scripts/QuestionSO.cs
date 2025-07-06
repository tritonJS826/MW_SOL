using UnityEngine;


[CreateAssetMenu(fileName = "NewQuestion", menuName = "ScriptableObject/Question")]
public class QuestionSO : ScriptableObject
{
    [SerializeField] private string questionName;
    [SerializeField] private string questionText;
    [SerializeField] private string correctAnswer;
    [SerializeField] private int speedTime;

    public string QuestionName => questionName;
    public string QuestionText => questionText;
    public string CorrectAnswer => correctAnswer;
    public int SpeedTime => speedTime;

}
