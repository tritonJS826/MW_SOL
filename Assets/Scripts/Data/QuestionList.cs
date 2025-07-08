namespace Data
{
    [System.Serializable]
    public class QuestionList
    {
        public QuestionData[] Questions { get; set; }

        public QuestionList(QuestionData[] questions)
        {
            Questions = questions;
        }
    }
}