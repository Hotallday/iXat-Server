using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

namespace iXat_Server {
    sealed internal class Server {

        //Just for now
        internal static DateTime StartTime = new DateTime(1970, 1, 1);
        //
        internal static readonly IList<Client> users = new List<Client>();
        internal static readonly List<string> badged = new List<string>();
        internal static List<string> ipbans = new List<string>();
        internal static readonly List<string> protectedc = new List<string>();

        internal static TcpListener ServerListener = null;
        internal static bool ListenForConnections = false;

        internal static void Initialize() {
            Console.Clear();
            //StartTime = DateTime.Now;
            try {
                Database.Connect();
                LoadConfig();
                StartListening();

                Console.Read();
            } catch (Exception ex) {
                Console.WriteLine("Failed to start the server");
                Console.WriteLine("Stack trace:", ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
        internal static async void StartListening() {
            ServerListener = new TcpListener(IPAddress.Any, 1243);
            ServerListener.Start();
            ListenForConnections = true;
            Console.WriteLine("[SERVER]-[INFO]: Mark server is listening for connections", Console.ForegroundColor = ConsoleColor.Yellow);
            while (true) {
                try {
                    var client = await ServerListener.AcceptTcpClientAsync();
                    ThreadPool.QueueUserWorkItem(async state => await new Client(client).Read());                  
                } catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                    ListenForConnections = false;
                    break;
                }
            }
        }
        internal static async void LoadConfig() {
            if (await Database.Open()) {
                var conf = await Database.FetchArray("SELECT * FROM server LIMIT 0, 1");
                Config.staff = JsonConvert.DeserializeObject<List<string>>((string)conf["staff"]);
                Config.helpers = JsonConvert.DeserializeObject<List<string>>((string)conf["helpers"]);
                Config.pawns = JsonConvert.DeserializeObject<List<string>>((string)conf["pawns"]);
                var pocount = await Database.query("SELECT count(distinct section) AS count FROM powers");
                Config.pcount = int.Parse(pocount.ToString());
                Console.WriteLine(Config.pcount);
                var IPbans = await Database.FetchArray("SELECT ipbans FROM server");
                ipbans = JsonConvert.DeserializeObject<List<string>>((string)IPbans["ipbans"]);
                /// Update database with process pid
                /// Will be used later.
                await Database.query($"UPDATE server SET pid='{Process.GetCurrentProcess().Id}'");
                ///
                await Database.Close();
            }
        }
      
        internal static void Disconnect(Client c) {
            //if (c.policy == 1) {
            //    foreach (var s in users) {
            //        s.Send($"<l u=\"{c.id}\" />\0");
            //    }
            //}
            users.Remove(c);
            c.Dispose();
        }
        internal static void Broadcast(string message, Client ignore = null) {
            if (ignore == null) {
                foreach (var c in users) {
                    c.Send(message);
                }
            }
            else {
                foreach (var c in users) {
                    if (c != ignore) {
                        c.Send(message);
                    }
                }
            }
        }
        internal static void PerformShutDown() {
            try {
                Console.WriteLine("Shutting down the server...");
                // TODO: Conclude safely, important running operations
                foreach (var c in users) {
                   // c.Notice("Server shutting down.");
                    c.Dispose();
                }
            //    ServerListener.Dispose();
            } finally {
                Environment.Exit(0);
            }
        }
    }
}