using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;

namespace DAL
{
    public class Repository : IRepository
    {
        private string _serverConnectionString
        {
            get {
                var env = Environment.GetEnvironmentVariable("DATABASE_URL") ?? @"postgres://postgres:mysecretpassword@localhost:5432/docker";
                var regex = new Regex("postgres://(\\w+):(\\w+)@([^\\:]+):(\\d+)/(\\w+)");
                var match = regex.Match(env);
                return $"Server={match.Groups[3]};Port={match.Groups[4]};User Id={match.Groups[1]};Password={match.Groups[2]};";
            }
        }
        
        private string _connectionString
        {
            get {
                var env = Environment.GetEnvironmentVariable("DATABASE_URL") ?? @"postgres://postgres:mysecretpassword@localhost:5432/docker";
                var regex = new Regex("postgres://(\\w+):(\\w+)@([^\\:]+):(\\d+)/(\\w+)");
                var match = regex.Match(env);
                return $"Server={match.Groups[3]};Port={match.Groups[4]};Database={match.Groups[5]};User Id={match.Groups[1]};Password={match.Groups[2]};";
            }
        }
        
        private string _databaseName
        {
            get {
                var env = Environment.GetEnvironmentVariable("DATABASE_URL") ?? @"postgres://postgres:mysecretpassword@localhost:5432/docker";
                var regex = new Regex("postgres://(\\w+):(\\w+)@([^\\:]+):(\\d+)/(\\w+)");
                var match = regex.Match(env);
                return match.Groups[5].ToString();
            }
        }

        public void SafeExecute(string connString, string query)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Execute(query);
                }
                catch (Exception e)
                {
                }
            }
        }
        
        public Repository()
        {
            SafeExecute(_serverConnectionString, $"create database {_databaseName};");
            
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Execute(@"CREATE TABLE IF NOT EXISTS public.Chats
                (
                    chat_id      INTEGER PRIMARY KEY NOT NULL,
                    dialog_state INTEGER NOT NULL,
                    latitude     FLOAT NULL,
                    longitude    FLOAT NULL,
                    area_id      INTEGER NULL,
                    timezone     INTEGER NULL,
                    weather      INTEGER NULL,
                    subscribed   BOOLEAN NULL
                );
                ");
            }
        }

        public void AddUserRecord(int chatId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Execute($"insert into Chats values ({chatId}, 0, null, null, null, null, null, null)");
            }
        }

        public UserRecord GetUserRecord(int chatId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                return conn.Query<UserRecord>($"select * from Chats where chat_id={chatId}").FirstOrDefault();
            }
        }

        public void UpdateUserRecord(UserRecord r)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Execute(
                    "update Chats set dialog_state = @dialog_state, latitude = @lat, longitude = @lon, area_id = @area, timezone = @tz, weather = @weather, subscribed = @subscribed where chat_id = @id", 
                    new {id = r.ChatId, lat = r.Latitude, lon = r.Longitude, area = r.AreaId, tz = r.Timezome, weather = r.Weather, subscribed = r.Subscribed, dialog_state = r.DialogState });
            }
        }
    }
}