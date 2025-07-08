using System;

namespace Data
{
    [Serializable]
    public class QuestionList
    {
        public QuestionData[] questions;

        public QuestionList(QuestionData[] questions)
        {
            this.questions = questions;
        }
    }
}