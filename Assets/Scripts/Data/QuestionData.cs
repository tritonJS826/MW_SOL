using System;

namespace Data
{
    [Serializable]
    public class QuestionData
    {
        public string Answer;
        public string Name;
        public int Order;
        public string QuestionText;
        public string Uuid;
        public float TimeToAnswer;
        
        public QuestionData(string answer, string name, int order, string questionText, string uuid, float timeToAnswer)
        {
            Answer = answer;
            Name = name;
            Order = order;
            QuestionText = questionText;
            Uuid = uuid;
            TimeToAnswer = timeToAnswer;
        }
        
        
    }
}
