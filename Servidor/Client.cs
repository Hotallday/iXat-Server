using iXat_Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Client  {
    public readonly Socket _client;

    public string bann, bride, rank, RealRank, id, username, nickname, k, k2, k3, password, avatar, url, powers, room, xats, days, chat, banned, pool;
    public string d0, d1, d2, d3, d4, d5, d6, dt, dx, dO, p0, p1, p2, p4, PowerO, d7, p3, homepage, h, group, pStr, pStr2;
    public string hash2 = "", loginkey = null, rExpired = "0", pawn = "";
    public int poolLimit = 60, app = 0, policy = 0;
    public List<string> last, last2, GroupPools, GroupPowers = new List<string>();
    public bool hidden = false, botOnline = false,sendJ2 = false, gotTickled = false, _null = false, switchingPools = false, joined = false, away = false, authenticated = false, online = false, disconnect = false, mobready = false, chatPass = false;
    public string handshake, cv;
    public byte[] buffer = new byte[1204];
    public uint[] l5 = new uint[] { 4286545791, 4286217085, 4285954170, 4285756791, 4285625204, 4285493618 };
    public Client(Socket client) {
        _client = client;
    }
    public void authenticate(Dictionary<string,string> packet) {

    }
   
}

