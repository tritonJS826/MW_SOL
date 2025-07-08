using System;

namespace Data
{
    [Serializable]
    public class QuestionData
    {
        public string answer;
        public string name;
        public int order;
        public string questionText;
        public string uuid;
        public float timeToAnswer;
        
        public QuestionData(string answer, string name, int order, string questionText, string uuid, float timeToAnswer)
        {
            this.answer = answer;
            this.name = name;
            this.order = order;
            this.questionText = questionText;
            this.uuid = uuid;
            this.timeToAnswer = timeToAnswer;
        }
        
        
    }
}
