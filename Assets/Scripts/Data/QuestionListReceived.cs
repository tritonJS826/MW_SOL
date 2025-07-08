namespace Data
{
    [System.Serializable]
    public class QuestionListReceived
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string QuestionText { get; set; }
        public string Uuid { get; set; }
        public float TimeToAnswer { get; set; }
        
        public QuestionListReceived(string name, int number, string questionText, string uuid, float timeToAnswer)
        {
            Name = name;
            Number = number;
            QuestionText = questionText;
            Uuid = uuid;
            TimeToAnswer = timeToAnswer;
        }
    }
}
