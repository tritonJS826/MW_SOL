using System;

namespace Data
{
    [Serializable]
    public class SessionStateUpdated
    {
        public UserInfo[] currentUsers;
        public string selfUserUuid;
        public string userHostUuid;
    }
}