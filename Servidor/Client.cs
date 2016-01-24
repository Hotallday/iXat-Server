using iXat_Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Client {
    public readonly Socket _client;

    public string bann, bride, rank, RealRank, id, username, nickname, password, avatar, url, powers, room, xats, days, chat, banned;
    public int d0, d1, d2, d3, d4, d5, d6, dt, dx, dO, p0, p1, p2, p4, PowerO, d7, p3, f, f2, k, k2, k3, pool;
    public string homepage, h, group, pStr, pStr2;
    public string hash2 = "", loginkey = null, rExpired = "0", pawn = "";
    public int poolLimit = 60, app = 0, policy = 0;
    public List<string> last, last2, GroupPools, GroupPowers = new List<string>();
    public bool hidden = false,b2,b, botOnline = false, sendJ2 = false, gotTickled = false, _null = false, switchingPools = false, joined = false, away = false, authenticated = false, online = false, disconnect = false, mobready = false, chatPass = false;
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
        id = (string)packet["u"];
        d0 = (int)packet["d0"];
        f = 0;
        f2 = (int)packet["f"];
        var n = (string)packet["N"];
        var k = (int)packet["k"];
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
        b2 = packet["b"] != null ? true : false;
        //b =  f & 8 ? true : false;
        var chat = (int)packet["c"];
    }

}

