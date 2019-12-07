using System;

namespace DAL
{
    public class Repository : IRepository
    {
        public void AddUserRecord(int chatId)
        {
            // sets dialog state = 0
        }

        public UserRecord GetUserRecord(int chatId)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserRecord(UserRecord r)
        {
            
        }
    }
}