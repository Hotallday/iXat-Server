using System;
using System.Collections.Generic;
partial class PacketHandler {
    ///TODO
    //Add mobile checking
    //Add Bot Protection
    ///
    private static async void J2(Dictionary<string, object> data, Client user) {
        if (!user.authenticated && user.loginKeys.Count > 0) {
            try {
                for (int i = 0; i < Config.pcount; i++) {
                    var d = $"d{i + 4}";
                    var m = $"m{i}";
                    if (data.ContainsKey((string)data[d]))
                        user.powersV["enabled"].Add(int.Parse((string)data[d]));
                    else
                        user.powersV["enabled"].Add(0);
                    if (data.ContainsKey((string)data[m]))
                        user.powersV["disabled"].Add(int.Parse((string)data[m]));
                    else
                        user.powersV["disabled"].Add(0);
                    user.userid = int.Parse((string)data["u"]);
                    if (data.ContainsKey("d0"))
                        user.d0 = int.Parse((string)data["u"]);
                    user.K = int.Parse((string)data["k"]);
                    if (data.ContainsKey("pool"))
                        user.pool = int.Parse((string)data["pool"]);
                    user.home = (string)data["h"];
                    user.avatar = (string)data["a"];
                    user.roomID = int.Parse((string)data["c"]);
                    user.nick = (string)data["n"];
                    if (data.ContainsKey("r"))
                        user.chatPass = (string)data["r"];
                    if (user.userid == 2) {
                        user.guest = true;
                        user.f = 0;
                    } else {
                        if (await Database.Open()) {
                            var u = await Database.FetchArray($"SELECT * FROM users WHERE id='{user.userid}' and k='{user.K}'");
                            if (u.Count <= 1) {
                                await Database.Close();
                                user.Dispose();
                                return;
                            }
                            user.guest = string.IsNullOrWhiteSpace((string)u["username"]);
                            user.userid = int.Parse((string)u["id"]);
                            user.d2 = int.Parse((string)u["d2"]);
                            user.xats = int.Parse((string)u["xats"]);
                            user.days = int.Parse((string)u["days"] ) - 1453848182 / 86400;
                            user.pawn = (string)u["custpawn"];
                            user.bride = int.Parse((string)u["bride"]);
                            user.uName = (string)u["username"];
                            user.uPass = (string)u["password"];
                            if(user.days > 0 ) {
                                //Get powers
                            }
                            if(!user.guest) {
                                await Database.query($"UPDATE users set nickname='{user.nick}', avatar='{user.avatar}', url='{user.home}', connectedlast='{user.client.Client.RemoteEndPoint}' WHERE id='{user.userid}'");
                            }
                            user.f = int.Parse((string)data["f"]);
                            user.authenticated = true;
                            await Database.Close();
                            await user.joinRoom(user.roomID);
                        }
                    }

                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                user.Dispose();
            }
        }
    }
}

