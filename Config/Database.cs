using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

public class Database {

    private static MySqlConnection Connection;
    public static string Url = "localhost";
    public static string User = "root";
    public static string Password = "ofeenel";
    public static string Db = "xatech";

    public static void Connect() {
        string connectionString = $"SERVER={Url};DATABASE={Db};UID={User};PASSWORD={Password};";
        Connection = new MySqlConnection(connectionString);
    }

    public static async Task<Dictionary<string, object>> FetchArray(string query) {
        var dictionary = new Dictionary<string, object>();
        using (var command = new MySqlCommand(query, Connection)) {
            using (var reader = await command.ExecuteReaderAsync()) {
                while (reader.Read()) {
                    for (var i = 0; i < reader.FieldCount; i++) {
                        var key = reader.GetName(i);
                        var value = reader[i];
                        dictionary.Add(key, value);
                    }
                }
            }
        }
        return dictionary;
    }
    public static async Task<object> query(string query) {
        using (var command = new MySqlCommand(query, Connection)) {
            return await command.ExecuteScalarAsync();
        }
    }

    public static async Task<bool> Open() {
        var openAsync = Connection?.OpenAsync();
        if (openAsync == null) return false;
        await openAsync;
        return true;
    }
    public static async Task<bool> Close() {
        var openAsync = Connection?.CloseAsync();
        if (openAsync == null) return false;
        await openAsync;
        return true;
    }
}


