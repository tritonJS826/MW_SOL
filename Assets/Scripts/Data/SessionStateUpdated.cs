using System;

namespace Data
{
    [Serializable]
    public class SessionStateUpdated
    {
        
        public UserInfo[] currentUsers;

        public SessionStateUpdated(UserInfo[] users)
        {
            this.currentUsers = users;
        }
    }
}