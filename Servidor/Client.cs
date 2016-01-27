using iXat_Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
/// <summary>
/// TODO FIX database queries. All of them allow SQL INJECTION
/// </summary>
public class Client {

    public bool alive = true, read = false, busy = false, mobile = false
        , hidden = false, banned = false, online = false, authenticated = false;
    public Dictionary<string, List<int>> powersV = new Dictionary<string, List<int>>() {
        {"enabled" , new List<int>()},

        {"disabled" , new List<int>()},
    };
    public string str = "";
    //UserInformation
    public int userid =0, roomID, d0 = 0, K =0, pool=0,f=0,d2 = 0,xats = 0,days =0,bride = 0;
    public bool guest = false;
    public string home, avatar, nick, chatPass,pawn,uName,uPass;
    //
    //Chat Info
    public int chat = 0,rank = 5;
    public string chatName, blastban, blastkick, blastpro, blastde;
    //
    public Dictionary<string, object> last, last2, GroupPools,loginKeys = new Dictionary<string, object>();
    public Dictionary<int, int> GroupPowers = new Dictionary<int, int>();

    public byte[] buffer = new byte[4354];
    public readonly TcpClient client;
    private readonly NetworkStream stream;
    public Client(TcpClient client) {
        client.ReceiveTimeout = 300;
        client.SendTimeout = 300;
        this.client = client;
        alive = true;
        read = true;
        stream = this.client.GetStream();
    }
    public async Task Read() {
        var mS = new MemoryStream();
        while (read) {
            try {
                var bytesrecv = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesrecv <= 0) {
                    Dispose();
                    break;
                }
                await mS.WriteAsync(buffer, 0, bytesrecv);
                var message = Encoding.UTF8.GetString(mS.ToArray());
                Console.WriteLine(message);
                Match findtype = PacketHandler.typeofpacket.Match(message);
                if (findtype.Success)
                    PacketHandler.HandlePacket[findtype.Groups[1].Value](Parse(message), this);
                mS.Seek(0, SeekOrigin.Begin);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Dispose();
                break;
            }
        }
    }

    //public void setFlag(object type) {
    //    switch ((string)type) {
    //        case "mobile":
    //            f |= 0x0200;
    //            break;
    //        case "banished":
    //        case "banish":
    //            f |= 0x1000;
    //            break;
    //        case "gagged":
    //        case "gag":
    //            f |= 256;
    //            break;
    //        case "invisible":
    //            f |= 1024;
    //            break;
    //        case "bot":
    //            f |= 8192;
    //            break;
    //        case "dunce":
    //            f |= 32768;
    //            break;
    //        case "typing":
    //            f |= 65563;
    //            break;
    //        case "badge":
    //            f |= 262144;
    //            break;
    //        default:
    //            f |= (int)type;
    //            break;
    //    }
    //}

    public void authenticate(Dictionary<string, object> packet) {
        ///Information from the j2 we don't neeed for now
        ///The packets are already in a Dictionary
        //var information = new string[] { "auth", "auth2", "u", "N", "k", "y", "fuckm1", "fuckm2", "huem3", "huem4", "h", "d0", "a", "c", "banned", "r", "b", "cv" };
        ///
        try {

        } catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
    }
    public async void Send(string data) {
        var datab = Encoding.UTF8.GetBytes(data);
        Console.WriteLine($"[SERVER]-[INFO]: Send -> {data}", Console.ForegroundColor = ConsoleColor.Green);
        await stream.WriteAsync(datab, 0, datab.Length);
    }
    public async Task joinRoom(int roomID, bool relog = true, bool nodup = false, int pool = 0, int bantick = 0, bool clickedPool = false) {
        try {
            if (!authenticated || roomID < 1) {
                Send("<logout t=\"Chat doesn't exist.\"");
                return;
            }
            if (await Database.Open()) {
                var chati = await Database.FetchArray($"SELECT * FROM chats WHERE id='{roomID}'");
                chat = int.Parse((string)chati["id"]);
                chatName = (string)chati["id"];
                blastban = (string)chati["blastban"];
                blastkick = (string)chati["blastkick"];
                blastpro = (string)chati["blastpro"];
                blastde = (string)chati["blastde"];
                if (userid == 2) {
                    rank = 5;
                } else {
                    var ranks = await Database.FetchArray($"SELECT f,tempend FROM ranks WHERE chatid='{chati["id"]}' AND userid='{userid}'");
                    //Check chat pass to get main owner
                    if (!string.IsNullOrWhiteSpace(chatPass)) {
                    }
                    //
                    if (!ranks.ContainsKey("f")) {
                        await Database.query($"INSERT INTO ranks(userid,chatid,f) VALUES({userid}, {chat}, 5)");
                        rank = 5;
                    } else if (int.Parse((string)ranks["tempend"]) > 0 && int.Parse((string)ranks["tempend"]) < (int)(DateTime.UtcNow - Server.StartTime).TotalSeconds) {
                        rank = 3;
                        await Database.query($"UPDATE ranks SET f=3, tempend=0 WHERE userid={userid} AND chatid={chat}");
                    } else {
                        rank = int.Parse((string)ranks["f"]);
                        if (int.Parse((string)ranks["f"]) < 6) rank = 5;
                    }
                }
                //Attached Chat Tab
                
                //
                Send("<i b=\"http://i.cubeupload.com/ftN0AD.png;=Deal;=22497272;=English;=http://87.230.56.15:80/;=#CCCC99;=\" f=\"21233730\" f2=\"4096\" cb=\"2317\" />\0");
                var iPack = new Dictionary<string, object>() {
                    { "b" ,$"{chati["bg"]};=;=;=;={chati["radio"]};={chati["button"]}"},
                    { "f" ,f},
                    { "v",3},
                    { "r",rank},
                    { "cb",(int)(DateTime.UtcNow - Server.StartTime).TotalSeconds}
                };
                var ipa = CreatePacket(iPack, "i");
                Console.WriteLine(ipa);
                ////if (!online) {
                ////    var gp = await buildGp();
                ////    Send(gp);
                ////}
                ////pool = 1;
                ////Send("<done />");
                await Database.Close();
            }
            return;
        } catch (Exception ex) {
            Console.WriteLine($"[SERVER]-[INFO]-[ERROR]: {ex}", Console.ForegroundColor = ConsoleColor.Red);
        }
        return;
    }
    //private string buildU() {
    //    var uPack = new Dictionary<string, object>();
    //    if (!string.IsNullOrWhiteSpace(pawn) && pawn.Length == 6) uPack.Add("pawn",pawn);
    //    if (!guest) uPack.Add("N", username);
    //    uPack.Add("f", f);
    //    uPack.Add("flag", f);
    //    uPack.Add("s", 1);
    //    uPack.Add("rank", rank);
    //    uPack.Add("u", id);
    //    uPack.Add("q", 3);
    //    uPack.Add("n", nickname);
    //    uPack.Add("a", avatar);
    //    uPack.Add("h", homepage);
    //    uPack.Add("d0", d0);
    //    uPack.Add("d2", d2);
    //    uPack.Add("bride", bride);
    //    uPack.Add("v", 2);
    //    uPack.Add("cv", (int)(DateTime.UtcNow - Server.StartTime).TotalSeconds);
    //    for (int i = 0; i < powers.Length; i++) {
    //        uPack.Add($"p{i}", powers[i]);
    //    }
    //    return CreatePacket(uPack, "u");
    //}
    //private async Task<string> buildGp() {
    //    var Already = new List<int>();
    //    var assigned = await Database.FetchArrayList($"SELECT power FROM `group_powers` WHERE `chat`='{this.group}' ");
    //    var group = await Database.FetchArray($"SELECT * FROM chats WHERE `name`='{this.group}'");
    //    var lastId = await Database.FetchArray($"SELECT section FROM powers ORDER BY id DESC LIMIT 1");
    //    var maxSect = Convert.ToInt16(lastId["section"].ToString().Replace("p", "")) + 1;
    //    for (int i = 0; i < maxSect; i++) {
    //        GroupPowers.Add(i, 0);
    //    }
    //    foreach (var row in assigned) {
    //        var power = await Database.FetchArray($"SELECT * FROM `powers` WHERE `id`='{row}'");
    //        if (power.ContainsKey("id")) {
    //            if (!Already.Contains(int.Parse(power["id"].ToString()))) {
    //                var subid = Convert.ToInt32(Math.Pow(2, Convert.ToInt32(power["id"]) % 32));
    //                var section = Convert.ToInt32(power["id"]) >> 5;
    //                GroupPowers[section] += subid;
    //                Already.Add(int.Parse(power["id"].ToString()));
    //            }
    //        }
    //    }
    //    var gp = new Dictionary<string, object>() {
    //        { "p",string.Join("|", GroupPowers.Values)},
    //        { "g80","{'mm':'14','mbt':48,'ss':'14','prm':'14','dnc':'14','bdg':'8'}"},
    //        { "g90", group["bad"]},
    //        { "g112", group["announce"]},
    //        { "g246","{'dt':70,'v':1" },
    //        { "g256", "{'rnk':'2','dt':65,'rt':15,'rc':'1','tg':200,'v':1}"},
    //        { "g92", group["hflix"]},
    //        { "g148", group["sflix"]},
    //        { "g114", group["pools"]},
    //        { "g100", group["link"]},
    //        {"g74",group["gline"] },
    //        {"g106", group["gback"] }
    //    };
    //    return CreatePacket(gp,"gp");
    //}
    public string CreatePacket(Dictionary<string, object> data, string name = "packet") {
        var str = $"<{name}";
        if (data.Count > 0) {
            str = data.Aggregate(str, (current, attr) => current + $" {attr.Key}=\"{attr.Value}\"");
        }
        str += " />\0";
        return str;
    }
    ////Dispose of more stuff if possible
    public void Dispose() {
        online = false;
        alive = false;
        read = false;
        if (Server.users.Contains(this))
            Server.users.Remove(this);
        client.GetStream().Close();
        client.Close();
        foreach (var users in Server.users)
            if(users != this)
                users.Send($"<l u=\"{userid}\" />\0");
    }
    public Dictionary<string, object> Parse(string packet) {
        try {
            var xml = new XmlDocument();
            xml.LoadXml(packet);
            var root = xml.DocumentElement;
            var attributes = new Dictionary<string, object> { };
            attributes["type"] = root?.Name;
            for (var x = 0; x < root?.Attributes.Count; x++) {
                attributes[root.Attributes[x].Name] = root.Attributes[x].Value;
            }
            return attributes;
        } catch (XmlException) {
            return null;
        }
    }
    //public void Notice(string message) {
    //    var datab = Encoding.ASCII.GetBytes($"<n t=\"{message}\" />\0");
    //    Console.WriteLine($"[SERVER]-[INFO]: Send -> {message}", Console.ForegroundColor = ConsoleColor.Green);
    //}


}


