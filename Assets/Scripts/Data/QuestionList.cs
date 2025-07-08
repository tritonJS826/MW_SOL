using System;

namespace Data
{
    [Serializable]
    public class QuestionList
    {
        public QuestionData[] Questions;

        public QuestionList(QuestionData[] questions)
        {
            Questions = questions;
        }
    }
}