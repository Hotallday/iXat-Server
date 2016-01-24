using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace iXat_Server {
    sealed internal class Server {
        internal static DateTime StartTime;
        internal static readonly IList<Client> users = new List<Client>();
        internal static readonly List<string> badged = new List<string>();
        internal static List<string> ipbans = new List<string>();
        internal static readonly List<string> protectedc = new List<string>();

        internal static Socket ServerListener = null;
        internal static void Initialize() {
            Console.Clear();
            StartTime = DateTime.Now;
            try {
                Database.Connect();
                LoadConfig();
                StartListening();
                
                Console.Read();
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to start the server");
                Console.WriteLine("Stack trace:", ex.Message);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
        internal static void StartListening() {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 1243);
            ServerListener = new Socket(ipe.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerListener.Bind(ipe);
            ServerListener.Listen(50);
            ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);
            Console.WriteLine("[SERVER]-[INFO]: Mark server is listening for connections", Console.ForegroundColor = ConsoleColor.Yellow);
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
                ipbans= JsonConvert.DeserializeObject<List<string>>((string)IPbans["ipbans"]);
                /// Update database with process pid
                /// Will be used later.
                await Database.query($"UPDATE server SET pid='{Process.GetCurrentProcess().Id}'");
                ///
                await Database.Close();
            }
        }
        internal static void ConnectCallBack(IAsyncResult ar) {
            var c = new Client(null);
            try {
                var s = (Socket)ar.AsyncState;
                c = new Client(s.EndAccept(ar));
                c._client.BeginReceive(c.buffer, 0, c.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), c);
                ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                if (c._client != null) {
                    c._client.Close();
                    lock (users) {
                        users.Remove(c);
                    }
                }
            }
            finally {
                ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);
            }
        }
        internal static void SendCallBack(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception ex) {
                Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex.Message}", Console.ForegroundColor = ConsoleColor.Red);
            }
        }
        internal static void ReceiveCallback(IAsyncResult result) {
            var c = (Client)result.AsyncState;
            try {
                if (c?._client == null)
                    return;
                var bytestoread = c._client.EndReceive(result);
                if (bytestoread > 0) {
                    string recv = Encoding.ASCII.GetString(c.buffer, 0, bytestoread);
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"[SERVER]-[INFO]: Received -> {recv}");
                    Match findtype = PacketHandler.typeofpacket.Match(recv);
                    if (findtype.Success)
                        PacketHandler.HandlePacket[findtype.Groups[1].Value](null, c);
                }
                c._client.BeginReceive(c.buffer, 0, c.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), c);
            }
            catch (Exception ex) {
                Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex.Message}", Console.ForegroundColor = ConsoleColor.Red);
            }
        }
        internal static void PerformShutDown() {
            Console.WriteLine("Shutting down the server...");
            // TODO: Conclude safely, important running operations
            Environment.Exit(0);
        }
        public static void Send(Socket soc, string data) {
            var datab = Encoding.ASCII.GetBytes(data);
            Console.WriteLine($"[SERVER]-[INFO]: Send -> {data}", Console.ForegroundColor = ConsoleColor.Green);
            soc.BeginSend(datab, 0, datab.Length, 0, new AsyncCallback(SendCallBack), soc);
        }
        public static string CreatePacket(Dictionary<string, object> data, string name = "packet") {
            var str = $"<{name}";
            if (data.Count > 0) {
                str = data.Aggregate(str, (current, attr) => current + $" {attr.Key}=\"{attr.Value}\"");
            }
            return str += " />\0";
        }
    }
}
