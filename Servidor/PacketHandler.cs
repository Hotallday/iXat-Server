using iXat_Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PacketHandler {
    private static readonly string PolicyReq = "<cross-domain-policy><allow-access-from domain='*' to-ports='*' /></cross-domain-policy>\0";
    public static Regex typeofpacket = new Regex(@"<([-\w]+)");
    public static Random rand = new Random();
    public static readonly IDictionary<string, Action<Dictionary<string, object>, Client>> HandlePacket = new Dictionary<string, Action<Dictionary<string, object>, Client>>
    {
        { "policy-file-request", Policy },
        { "y",Y},
        { "j2", J2}
    };

    private static void J2(Dictionary<string, object> arg1, Client arg2) {
        if (arg2.authenticated == true) {
            //return disconnect user
        }
        arg2.authenticate(arg1);
    }

    private static void Y(Dictionary<string, object> arg1, Client arg2) {
        //php time()
        var time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        arg2.sendJ2 = false;
        //I'm using a custom client so this values are different from the main client
        var ma = $"[24,15,45,45]";
        Dictionary<string, object> YPack = new Dictionary<string, object>
        {
            { "yi",rand.Next(10000000, 99999999) },
            { "ys",rand.Next(2,5) },
            { "yc", time},
            { "yk", time },
            { "ya",ma},
            { "yp","100_100_5_13821"}
        };
        Server.Send(arg2._client, Server.CreatePacket(YPack, "y"));
    }
    private static void Policy(Dictionary<string, object> arg1, Client arg2) {
        if (arg2.policy == 0) {
            Server.Send(arg2._client, PolicyReq);
            arg2.policy = 1;
        }
    }
}

