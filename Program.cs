using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace iXat_Server {
    class Program {
        
        public static Regex regex = new Regex(@"<([-\w]+)");
        private static ushort limitofconnections = 100;
        static void Main(string[] args) {
            Server.Start();
            Console.Read();
        }

        public static string createPacket(Dictionary<string, string> data, string name = "packet") {
            var str = $"<{name}";
            if (data.Count > 0) {
                foreach (var attr in data) {
                    str += $" {attr.Key}=\"{attr.Value}\"";
                }
            }
            return str += " />";
        }
    }
}