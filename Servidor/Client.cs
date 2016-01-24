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
    public Dictionary<string, object> last, last2, GroupPools = new Dictionary<string, object>();
    public Dictionary<int, int> GroupPowers = new Dictionary<int, int>();
    public bool hidden = false, b2, b, guest = true, botOnline = false, sendJ2 = false, gotTickled = false, _null = false, switchingPools = false, joined = false, away = false, authenticated = false, online = false, disconnect = false, mobready = false, chatPass = false;
    public string handshake, cv;
    public byte[] buffer = new byte[1024];
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
                this.pool = pool;
                this.chat = chatinfo["id"].ToString();
                //this.chatid (int)chatinfo["id"] },
                group = (string)chatinfo["name"];
                hidden = false;
               // {"HasGroupPools", false},
               // {"hidden", false}

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
                var gp = await buildGp();
                Server.Send(_client, gp);
                foreach (var users in Server.users) {
                    var pawn2 = users.pawn.Length == 6 ? $" pawn=\"{users.pawn}\"" : "";
                    var mnick2 = $"Testing{PacketHandler.rand.Next(0, 110)}";
                    var userU = $"<u{pawn2} f=\"{users.f}\" flag=\"{users.f}\" rank=\"{users.RealRank}\" u=\"{users.id}\" q=\"3\" {(string.IsNullOrWhiteSpace(users.username) ? "" : $"N=\"{users.username}\"")} n=\"{mnick2}\" a=\"{users.avatar}\" h=\"{users.url}\" d0=\"{users.d0}\" d2=\"{users.d2}\" bride=\"{users.bride}\" {users.pStr}v=\"2\" cb=\"{(int)(DateTime.UtcNow - Server.StartTime).TotalSeconds}\" />\0";
                    Server.Send(_client, userU);
                    Server.Send(users._client, myPackity);
                }
                //Server.Broadcast(myPackity, this);
                Server.Send(_client, "<done />");
                await Database.Close();
            }
            return true;
        } catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
        return false;
    }
    private async Task<string> buildGp() {
        var Already = new List<int>();
        var assigned = await Database.FetchArrayList($"SELECT power FROM `group_powers` WHERE `chat`='{this.group}' ");
        var group = await Database.FetchArray($"SELECT * FROM chats WHERE `name`='{this.group}'");
        var lastId = await Database.FetchArray($"SELECT section FROM powers ORDER BY id DESC LIMIT 1");
        var maxSect = Convert.ToInt16(lastId["section"].ToString().Replace("p", "")) + 1;
        for (int i = 0; i < maxSect; i++) {
            GroupPowers.Add(i, 0);
        }
        foreach (var row in assigned) {
            var power = await Database.FetchArray($"SELECT * FROM `powers` WHERE `id`='{row}'");
            if (power.ContainsKey("id")) {
                if (!Already.Contains(int.Parse(power["id"].ToString()))) {
                    var subid = Convert.ToInt32(Math.Pow(2, Convert.ToInt32(power["id"]) % 32));
                    var section = Convert.ToInt32(power["id"]) >> 5;
                    GroupPowers[section] += subid;
                    Already.Add(int.Parse(power["id"].ToString()));
                }
            }
        }
        var gp = new Dictionary<string, object>() {
            { "p",string.Join("|", GroupPowers.Values)},
            { "g80","{'mm':'14','mbt':48,'ss':'14','prm':'14','dnc':'14','bdg':'8'}"},
            { "g90", group["bad"]},
            { "g112", group["announce"]},
            { "g246","{'dt':70,'v':1" },
            { "g256", "{'rnk':'2','dt':65,'rt':15,'rc':'1','tg':200,'v':1}"},
            { "g92", group["hflix"]},
            { "g148", group["sflix"]},
            { "g114", group["pools"]},
            { "g100", group["link"]},
            {"g74",group["gline"] },
            {"g106", group["gback"] }
        };
        return Server.CreatePacket(gp,"gp");
    }
    //Dispose of more stuff if possible
    public void Dispose() {
        _client.Dispose();
    }
}

