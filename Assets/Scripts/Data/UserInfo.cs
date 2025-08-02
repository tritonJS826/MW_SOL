using System;

namespace Data
{
    [Serializable]
    public class UserInfo
    {
        public string userUuid;

        public string userName;

        public UserInfo(string userUuid, string userName)
        {
            this.userUuid = userUuid;
            this.userUuid = userName;
        }
        
        
    }
}
