using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System;

namespace iXat_Server.Config
{
    public class MySql
    {

        private static MySqlConnection Connection;
        public static string Url;
        public static string User;
        public static string Password;
        public static string Database;

        public static void Connect()
        {
            string connectionString = $"SERVER={Url};DATABASE={Database};UID={User};PASSWORD={Password};";
            Connection = new MySqlConnection(connectionString);
        }

        public static async Task<MySqlConnection> getConnection()
        {
            Connect();
            if (await Open())
            {
                return Connection;
            }
            throw new Exception();
        }

        public static async Task<Dictionary<string, object>> FetchArray(string query)
        {
            var dictionary = new Dictionary<string, object>();
            using (var command = new MySqlCommand(query, Connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var key = reader.GetName(i);
                            var value = reader[i];
                            dictionary.Add(key, value);
                        }
                    }
                }
            }
            return dictionary;
        }

        public static async Task<bool> Open()
        {
            var openAsync = Connection?.OpenAsync();
            if (openAsync == null) return false;
            await openAsync;
            return true;
        }

        public static async Task<bool> Close()
        {
            var openAsync = Connection?.CloseAsync();
            if (openAsync == null) return false;
            await openAsync;
            return true;
        }       
    }
}

