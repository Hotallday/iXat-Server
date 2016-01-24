using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;

namespace iXat_Server {
    sealed internal class Server
    {
        internal static DateTime StartTime;
        internal static readonly IList<Client> Users = new List<Client>();
        internal static Socket ServerListener = null;

        /// <summary>
        /// Initializes the server
        /// </summary>
        internal static void Initialize() {
            Console.Clear();

            StartTime = DateTime.Now;

            try
            {
                StartListening();

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start the server");
                Console.WriteLine("Stack trace:", ex.Message);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        internal static void StartListening()
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 1243);
            ServerListener = new Socket(ipe.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerListener.Bind(ipe);
            ServerListener.Listen(50);
            ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);

            Console.WriteLine("[SERVER]-[INFO]: Mark server is listening for connections", Console.ForegroundColor = ConsoleColor.Yellow);
        }

        internal static void ConnectCallBack(IAsyncResult ar) {
            var c = new Client(null);

            try
            {
                var s = (Socket)ar.AsyncState;              
                c = new Client(s.EndAccept(ar));
                c._client.BeginReceive(c.buffer, 0, c.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), c);
                ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                if (c._client != null) {
                    c._client.Close();
                    lock (Users) {
                        Users.Remove(c);
                    }
                }
            } finally {
                ServerListener.BeginAccept(new AsyncCallback(ConnectCallBack), ServerListener);
            }
        }

        internal static void SendCallBack(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal static void ReceiveCallback(IAsyncResult result) {
            var c = (Client)result.AsyncState;

            try
            {
                if (c?._client == null)
                    return;

                var bytestoread = c._client.EndReceive(result);

                if (bytestoread > 0) {
                    string g = Encoding.ASCII.GetString(c.buffer,0,bytestoread);

                    Console.WriteLine(g);
                    Match findtype = PacketHandler.typeofpacket.Match(g);

                    if (findtype.Success)
                        PacketHandler.HandlePacket[findtype.Groups[1].Value](null, c);                          
                }

                c._client.BeginReceive(c.buffer, 0, c.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), c);
            }
            catch
            {
                // ignored
            }
        }

        internal static void PerformShutDown()
        {
            Console.WriteLine("Shutting down the server...");

            // TODO: Conclude safely, important running operations

            Environment.Exit(0);
        }

        public static void Send(Socket soc, string data)
        {
            var datab = Encoding.ASCII.GetBytes(data);
            soc.BeginSend(datab, 0, datab.Length, 0, new AsyncCallback(SendCallBack), soc);
        }

        public static string CreatePacket(Dictionary<string, string> data, string name)
        {
            string str = $"<{name}";

            if (data.Count > 0)
            {
                str = data.Aggregate(str, (current, attr) => current + $" {attr.Key}=\"{attr.Value}\"");
            }

            return str += " />\0";
        }
    }
}
