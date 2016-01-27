using iXat_Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
partial class PacketHandler {
    private static readonly string PolicyReq = "<cross-domain-policy><allow-access-from domain='*' to-ports='*' /></cross-domain-policy>\0";
    public static Regex typeofpacket = new Regex(@"<([-\w]+)");
    public static Random rand = new Random();
    public static readonly IDictionary<string, Action<Dictionary<string, object>, Client>> HandlePacket = new Dictionary<string, Action<Dictionary<string, object>, Client>>
    {
        { "policy-file-request", Policy },
        { "y",Y},
        { "j2", J2},
        { "f", F },
        { "z",Z},
        {"m", M },
        {"c",C }
    };
}

