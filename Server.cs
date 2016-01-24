using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iXat_Server {
    class Server {
        private static readonly IList<Client> Users = new List<Client>();
        private static Socket serverListener = null;

        public static void Start() {
            try {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 1243);
                serverListener = new Socket(ipe.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverListener.Bind(ipe);
                serverListener.Listen(50);
                serverListener.BeginAccept(new AsyncCallback(ConnectCallBack), serverListener);
                Console.WriteLine("[SERVER]-[INFO]: Mark server is listening for connections", Console.ForegroundColor = ConsoleColor.Yellow);
            }
            catch (Exception ex) {
                Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex.Message}", Console.ForegroundColor = ConsoleColor.Red);
            }
        }

        private static void ConnectCallBack(IAsyncResult ar) {
            var c = new Client(null);
            try {
                var s = (Socket)ar.AsyncState;              
                c = new Client(s.EndAccept(ar));
                c._client.BeginReceive(c.buffer, 0, c.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), c);
                serverListener.BeginAccept(new AsyncCallback(ConnectCallBack), serverListener);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                if (c._client != null) {
                    c._client.Close();
                    lock (Users) {
                        Users.Remove(c);
                    }
                }
            }
            finally {
                serverListener.BeginAccept(new AsyncCallback(ConnectCallBack), serverListener);
            }
        }
        public static void Send(Socket soc,string data) {
            var datab = Encoding.ASCII.GetBytes(data);
            soc.BeginSend(datab, 0, datab.Length, 0, new AsyncCallback(SendCallBack), soc);
        }

        private static void SendCallBack(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult result) {
            var C = (Client)result.AsyncState;
            try {
                if (C == null || C._client == null) return;
                var bytestoread = C._client.EndReceive(result);
                if(bytestoread > 0) {
                    string g = Encoding.ASCII.GetString(C.buffer,0,bytestoread);
                    Match findtype = PacketHandler.typeofpacket.Match(g);
                    if (findtype.Success)
                        PacketHandler.HandlePacket[findtype.Groups[1].Value](null, C);                          
                    Console.WriteLine(g);
                }
                C._client.BeginReceive(C.buffer, 0, C.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), C);
            }
            catch {
               
            }
        }
    }
}
