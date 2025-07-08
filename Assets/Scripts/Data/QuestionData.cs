namespace Data
{
    [System.Serializable]
    public class QuestionData
    {
        public string Answer { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string QuestionText { get; set; }
        public string Uuid { get; set; }
        public float TimeToAnswer { get; set; }
        
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
