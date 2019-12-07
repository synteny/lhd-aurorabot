using System.Collections.Generic;

namespace DAL
{
    public interface IRepository
    {
        void AddUserRecord(int chatId);
        UserRecord GetUserRecord(int chatId);
        void UpdateUserRecord(UserRecord r);
        IEnumerable<UserRecord> GetAllUserRecords();
    }
}