using System;

namespace Data
{
    [Serializable]
    public class QuestionList
    {
        
        public QuestionData[] questions;

        public QuestionList()
        {
            // Optional: Initialize the array here as a good default,
            // though the object initializer will overwrite it immediately after.
            // this.questions = new QuestionData[0];
        }

        public QuestionList(QuestionData[] questions)
        {
            this.questions = questions;
        }
    }
}