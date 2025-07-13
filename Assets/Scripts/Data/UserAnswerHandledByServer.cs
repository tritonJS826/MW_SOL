namespace Data
{
    [System.Serializable]
    public class UserAnswerHandledByServer
    {
        public bool isOk { get; set; }
        public string userUuid { get; set; }
        public string userAnswer { get; set; }
        public string questionName { get; set; }
        public string questionDescription { get; set; }
        public string questionAnswer { get; set; }
        public string resultDescription { get; set; }
        public string questionUuid { get; set; }
        public string uuid { get; set; }


        public UserAnswerHandledByServer(bool isOk, string userUuid, string userAnswer, string questionName,
            string questionDescription, string questionAnswer, string resultDescription, string questionUuid,
            string uuid)
        {
            this.isOk = isOk;
            this.userUuid = userUuid;
            this.userAnswer = userAnswer;
            this.questionName = questionName;
            this.questionDescription = questionDescription;
            this.questionAnswer = questionAnswer;
            this.resultDescription = resultDescription;
            this.questionUuid = questionUuid;
            this.uuid = uuid;
        }
    }
}