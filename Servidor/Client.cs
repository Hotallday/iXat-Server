using iXat_Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/// <summary>
/// TODO FIX database queries. All of them allow SQL INJECTION
/// </summary>
public class  Client : IDisposable {
    public readonly Socket _client;

    public string bann, bride, RealRank, username, nickname, password, avatar, url, powers, room, xats, days, chat, banned;
    public int d0, d1, d2, d3, d4, d5, d6, dt, dx, dO, p0, p1, p2, p4, PowerO, d7, p3, f, f2, k, k2, k3, pool, rank, id;
    public string homepage, h, group, pStr, pStr2;
    public string hash2 = "", loginkey = null, pawn = "";
    public int poolLimit = 60, app = 0, policy = 0, rExpired = 0;
    public Dictionary<string, object> last, last2, GroupPools, GroupPowers = new Dictionary<string, object>();
    public bool hidden = false, b2, b, guest = true, botOnline = false, sendJ2 = false, gotTickled = false, _null = false, switchingPools = false, joined = false, away = false, authenticated = false, online = false, disconnect = false, mobready = false, chatPass = false;
    public string handshake, cv;
    public byte[] buffer = new byte[1204];
    public uint[] l5 = new uint[] { 4286545791, 4286217085, 4285954170, 4285756791, 4285625204, 4285493618 };
    public Client(Socket client) {
        _client = client;
    }
    public void setFlag(object type) {
        switch ((string)type) {
            case "mobile":
                f |= 0x0200;
                break;
            case "banished":
            case "banish":
                f |= 0x1000;
                break;
            case "gagged":
            case "gag":
                f |= 256;
                break;
            case "invisible":
                f |= 1024;
                break;
            case "bot":
                f |= 8192;
                break;
            case "dunce":
                f |= 32768;
                break;
            case "typing":
                f |= 65563;
                break;
            case "badge":
                f |= 262144;
                break;
            default:
                f |= (int)type;
                break;
        }
    }

    public void authenticate(Dictionary<string, object> packet) {
        ///Information from the j2 we don't neeed for now
        ///The packets are already in a Dictionary
        //var information = new string[] { "auth", "auth2", "u", "N", "k", "y", "fuckm1", "fuckm2", "huem3", "huem4", "h", "d0", "a", "c", "banned", "r", "b", "cv" };
        ///
        try {
            for (int i = 0; i < Config.pcount; i++) {
                if (packet.ContainsKey($"d{i + 4}")) {
                    pStr += $" p{i}=\"{packet[$"d{i + 4}"]}\" ";
                } else {
                    pStr += $" p{i}=\"0\" ";
                }
            }
            id = int.Parse(packet["u"].ToString());
            if (packet.ContainsKey("d0")) {
                d0 = (int)packet["d0"];
            }
            f = 0;
            f2 = int.Parse(packet["f"].ToString());
            var n = "";
            if (packet.ContainsKey("N")) {
                 n = (string)packet["N"];
            }
            var k = int.Parse(packet["k"].ToString());
            this.k = k;
            if (!packet.ContainsKey("pool")) {
                var pool = this.pool;
            } else {
                var pool = (int)packet["pool"];
                this.pool = pool;
            }
            ///TODO 
            ///Check if user is using mobile and add the mobile flag
            ///Check if user is badged and add the badged flag
            ///
            // b2 = packet["b"] != null ? true : false;
            //b =  f & 8 ? true : false;
            var chat = int.Parse(packet["c"].ToString());
            authenticated = true;

            joinRoom(chat, true, true, 1, 0, false).Wait();
        } catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
    }
    public async Task<object> joinRoom(int chat, bool relog = true, bool nodup = false, int pool = 0, int bantick = 0, bool clickedPool = false) {
        try {
            if (!authenticated || chat < 1) {
                return false;
            }
            if (await Database.Open()) {
                var chatinfo = await Database.FetchArray($"SELECT * FROM chats WHERE id={chat}");
                var items = new Dictionary<string, object>() {
                {"pool",pool},
                {"chat",chatinfo["id"]},
                {"chatid",chatinfo["id"] },
                {"group", chatinfo["name"]},
                {"HasGroupPools", false},
                {"hidden", false}
            };
                var ranks = await Database.FetchArray($"SELECT * FROM ranks WHERE chatid='{chatinfo["id"]}' AND userid='{id}'");
                ///Login to chat panel to get main owner
                if (chatPass) {

                }
                ///
                /// New user gets quest rank
                /// TODO Stop using php time()
                if (!ranks.ContainsKey("f")) {
                    ranks["f"] = 5;
                    await Database.query($"INSERT INTO ranks(userid,chatid,f) VALUES({id}, {chatinfo["id"]}, 5)");
                }
                ///
                ///Temp Rank
                else if (int.Parse(ranks["tempend"].ToString()) > 0 && int.Parse(ranks["tempend"].ToString()) < (int)(DateTime.UtcNow - Server.StartTime).TotalSeconds) {
                    ranks["f"] = 3;
                    await Database.query($"UPDATE ranks SET f=3, tempend=0 WHERE userid={id} AND chatid={chatinfo["id"]}");
                } else {
                    var userrank = ranks["f"];
                    rExpired = int.Parse(ranks["tempend"].ToString()) > (int)(DateTime.UtcNow - Server.StartTime).TotalSeconds ? int.Parse(ranks["tempend"].ToString()) : 0;
                }
                rank = (int)ranks["f"];
                ///
                //Check if user is null Null id is 2
                if (id == 2) {
                    id = (int)(DateTime.UtcNow - Server.StartTime).TotalSeconds + 000000;
                    _null = true;
                    username = null;
                    k2 = 0;
                    k = 0;
                    k3 = 0;
                    guest = true;
                } else {
                    //updateDetails
                    //resetDetails
                    _null = false;
                }
                Server.Send(_client, "<i b=\"http://i.cubeupload.com/ftN0AD.png;=Deal;=22497272;=English;=http://87.230.56.15:80/;=#CCCC99;=\" f=\"21233730\" f2=\"4096\" cb=\"2317\" />\0");
                chat = int.Parse(chatinfo["id"].ToString());
                var pawn = this.pawn.Length == 6 ? $" pawn=\"{this.pawn}\"" : "";
                var mnick = $"Testing{PacketHandler.rand.Next(0,110)}";
                var myPackity = $"<u{pawn} f=\"{f}\" flag=\"{f}\" rank=\"{RealRank}\" u=\"{id}\" q=\"3\" {(string.IsNullOrWhiteSpace(username) ? "" : $"N=\"{username}\"")} n=\"{mnick}\" a=\"{avatar}\" h=\"{url}\" d0=\"{d0}\" d2=\"{d2}\" bride=\"{bride}\" {pStr}v=\"2\" cb=\"{(int)(DateTime.UtcNow - Server.StartTime).TotalSeconds}\" />\0";
                Server.Broadcast(myPackity,_client);
                var gp = await buildGp();
                Console.WriteLine(gp);
                //Server.Send(_client, gp);
                await Database.Close();
            }
            return true;
        } catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
        return false;
    }
    private async  Task<string> buildGp() {
        Console.WriteLine(this.group);
        var assigned = await Database.FetchArray($"SELECT * FROM group_powers WHERE chat={this.group}");
        var group = await Database.FetchArray($"SELECT * FROM chats WHERE name={this.group}");
        var lastId = await Database.FetchArray($"SELECT * FROM powers ORDER BY id DESC LIMIT 1");
        var maxSect = lastId["section"].ToString().Replace("p", "") + 1;
        foreach(var row in assigned) {
            Console.WriteLine("row");
        }
        return "";
    }
    //Dispose of more stuff if possible
    public void Dispose() {
        _client.Dispose();
    }
}

