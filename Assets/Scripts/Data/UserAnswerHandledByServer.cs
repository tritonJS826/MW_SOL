namespace Data
{
    [System.Serializable]
    public class UserAnswerHandledByServer
    {
        public bool IsOk { get; set; }
        public string UserUuid { get; set; }
        public string UserAnswer { get; set; }
        public string QuestionName { get; set; }
        public string QuestionDescription { get; set; }
        public string QuestionAnswer { get; set; }
        public string ResultDescription { get; set; }
        public string QuestionUuid { get; set; }
        public string Uuid { get; set; }


        public UserAnswerHandledByServer(bool isOk, string userUuid, string userAnswer, string questionName,
            string questionDescription, string questionAnswer, string resultDescription, string questionUuid,
            string uuid)
        {
            IsOk = isOk;
            UserUuid = userUuid;
            UserAnswer = userAnswer;
            QuestionName = questionName;
            QuestionDescription = questionDescription;
            QuestionAnswer = questionAnswer;
            ResultDescription = resultDescription;
            QuestionUuid = questionUuid;
            Uuid = uuid;
        }
    }
}