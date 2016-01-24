using iXat_Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PacketHandler {
    private static readonly string PolicyReq = "<cross-domain-policy><allow-access-from domain='*' to-ports='*' /></cross-domain-policy>\0";
    public static Regex typeofpacket = new Regex(@"<([-\w]+)");
    public static Random rand = new Random();
    public static readonly IDictionary<string, Action<Dictionary<string, string>, Client>> HandlePacket = new Dictionary<string, Action<Dictionary<string, string>, Client>>
    {
        { "policy-file-request", Policy },
        { "y",Y},
       // { "j2", J2}
    };

    private static void J2(Dictionary<string, string> arg1, Client arg2) {
     //   if(arg2.authenticated == true) {
            // return disconnect user
       // }
       
    }

    private static void Y(Dictionary<string, string> arg1, Client arg2) {
        //php time()
        var time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        arg2.sendJ2 = false;
        //I'm using a custom client so this values are different from the main client
        var ma = $"[24,15,45,45]";
        Dictionary<string, string> YPack = new Dictionary<string, string>
        {
            { "yi",rand.Next(10000000, 99999999).ToString() },
            { "ys",rand.Next(2,5).ToString() },
            { "yc", time.ToString()},
            { "yk", time.ToString() },
            { "ya",ma },
            { "yp","100_100_5_13821"}
        };
        Server.Send(arg2._client, Server.createPacket(YPack, "y"));
    }
    private static void Policy(Dictionary<string, string> arg1, Client arg2) {
        if (arg2.policy == 0) {
            Server.Send(arg2._client, PolicyReq);
            arg2.policy = 1;
        }
    }
}

