using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public interface IRepository
    {
        void AddUserRecord(int chatId);
        UserRecord GetUserRecord(int chatId);
        void UpdateUserRecord(UserRecord r);
        IEnumerable<UserRecord> GetAllUserRecords();
    }

    public class HackedRepository : IRepository
    {
        private List<UserRecord> users = new List<UserRecord>();
        
        public void AddUserRecord(int chatId)
        {
            users.Add(new UserRecord{ChatId = chatId});
        }

        public UserRecord GetUserRecord(int chatId)
        {
            return users.FirstOrDefault(_ => _.ChatId == chatId);
        }

        public void UpdateUserRecord(UserRecord r)
        {
            users.Prepend(r);
        }

        public IEnumerable<UserRecord> GetAllUserRecords()
        {
            return users;
        }
    }
}