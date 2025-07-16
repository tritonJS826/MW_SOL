using System;

namespace Data
{
    [Serializable]
    public class UserInfo
    {
        public string userUuid;
        
        public UserInfo(string userUuid)
        {
            this.userUuid = userUuid;
        }
        
        
    }
}
