using Mirai_CSharp.Models;
using System;
using System.Threading.Tasks;
using alice_bot_cs_sw.Core;
using Mirai_CSharp;
using Mirai_CSharp.Plugin;

namespace alice_bot_cs_sw.Plugins
{
    public partial class ReconnectControl : IPlugin
    {
        private string _host = "127.0.0.1";
        private int _port = 8080;
        private string _authkey = "1145141919810";
        private long _qq = 0;
        
        public ReconnectControl(string host,int port, string authkey, long qq)
        {
            this._host = host;
            this._port = port;
            this._authkey = authkey;
            this._qq = qq;
        }
        public async Task<bool> Disconnected(MiraiHttpSession session, IDisconnectedEventArgs e)
        {
            Log.LogOut("exception-", e.Exception.Message);
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(_host, _port, _authkey);
            while (true)
            {
                try
                {
                    await session.ConnectAsync(options, _qq);
                    return true;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}