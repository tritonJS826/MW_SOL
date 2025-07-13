using System;

namespace Data
{
    [Serializable]
    public class UserAnswerHandledByServer
    {
        public bool isOk;
        public string userUuid;
        public string userAnswer;
        public string questionName;
        public string questionDescription;
        public string questionAnswer;
        public string resultDescription;
        public string questionUuid;
        public string uuid;

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