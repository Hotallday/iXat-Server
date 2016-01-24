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
public class Client {
    public readonly Socket _client;

    public string bann, bride, rank, RealRank, id, username, nickname, password, avatar, url, powers, room, xats, days, chat, banned;
    public int d0, d1, d2, d3, d4, d5, d6, dt, dx, dO, p0, p1, p2, p4, PowerO, d7, p3, f, f2, k, k2, k3, pool;
    public string homepage, h, group, pStr, pStr2;
    public string hash2 = "", loginkey = null, rExpired = "0", pawn = "";
    public int poolLimit = 60, app = 0, policy = 0;
    public List<string> last, last2, GroupPools, GroupPowers = new List<string>();
    public bool hidden = false, b2, b, botOnline = false, sendJ2 = false, gotTickled = false, _null = false, switchingPools = false, joined = false, away = false, authenticated = false, online = false, disconnect = false, mobready = false, chatPass = false;
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
                    pStr += $"p{i}=\"{packet[$"d{i + 4}"]}\"";
                }
                else {
                    pStr += "p{i}=\"0\"";
                }
            }
            id = (string)packet["u"];
            if (packet.ContainsKey("d0")) {
                d0 = (int)packet["d0"];
            }
            f = 0;
            f2 = int.Parse(packet["f"].ToString());
            var n = (string)packet["N"];
            var k = int.Parse(packet["k"].ToString());
            this.k = k;
            if (!packet.ContainsKey("pool")) {
                var pool = this.pool;
            }
            else {
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
        }
        catch (Exception ex) {
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
                if (!ranks.ContainsKey("f")) {
                    ranks["f"] = 5;
                    await Database.query($"INSERT INTO ranks(userid,chatid,f) VALUES({id}, {chatinfo["id"]}, 5)");
                }
                ///
                await Database.Close();
            }
            return true;
        }
        catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
        return false;
    }
}

