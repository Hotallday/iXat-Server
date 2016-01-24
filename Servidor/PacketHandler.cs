using iXat_Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PacketHandler {
    private static readonly string PolicyReq = "<cross-domain-policy><allow-access-from domain='*' to-ports='*' /></cross-domain-policy>\0";
    public static Regex typeofpacket = new  Regex(@"<([-\w]+)");
    public static readonly IDictionary<string, Action<Dictionary<string, string>, Client>> HandlePacket = new Dictionary<string, Action<Dictionary<string, string>, Client>>
    {
            { "policy-file-request", policy }
        };

    private static void policy(Dictionary<string, string> arg1, Client arg2) {
        Server.Send(arg2._client, PolicyReq);
    }
}

