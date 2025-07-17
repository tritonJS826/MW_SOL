using System;

namespace Data
{
    [Serializable]
    public class SessionStateUpdated
    {
        
        public UserInfo[] currentUsers;
        public string selfUserUuid;

        public SessionStateUpdated(UserInfo[] users)
        {
            this.currentUsers = users;
            this.selfUserUuid = selfUserUuid;
        }
    }
}