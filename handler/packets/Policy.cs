using System.Collections.Generic;
partial class PacketHandler {
    private static  void Policy(Dictionary<string, object> arg1, Client arg2) {
        //if (arg2.policy == 0) {
            arg2.Send(PolicyReq);
        //    arg2.policy = 1;
        //}
    }
}

