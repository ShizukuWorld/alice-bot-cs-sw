using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using alice_bot_cs_sw.Core;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace alice_bot_cs_sw.Plugins
{
    public partial class BotBehaviourControl : IBotInvitedJoinGroup, INewFriendApply, IGroupMessage
    {
        /// <summary>
        /// 机器人行为控制插件
        /// </summary>
        private string _help;
        private long _botqq;

        /// <summary>
        /// 无参数构造方法，不允许实例化
        /// </summary>
        private BotBehaviourControl()
        {
        }

        /// <summary>
        /// 有参数构造方法，加载时调用
        /// </summary>
        /// <param name="botqq">机器人QQ号</param>
        public BotBehaviourControl(long botqq)
        {
            this._botqq = botqq;
            CreateBotBehaviourControlConfig();
        }

        /// <summary>
        /// 初始化本插件所需要的config
        /// </summary>
        private static void CreateBotBehaviourControlConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + @"/config/";
            string configFilePath = configPath + @"BotBehaviourConfig.yaml";
            if (false == System.IO.File.Exists(configFilePath))
            {
                FileStream fs = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();
                var botBehaviourConfig = new BotBehaviourConfig
                {
                    request = new Request
                    {
                        friendRequest = "t",
                        groupRequest = "t",
                    },
                    menu = new Menu
                    {
                        help = "Alice Core Menu \n使用教程:https://www.yuque.com/mashirosa/public-note/alicebot",
                    }
                };
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                var yaml = serializer.Serialize(botBehaviourConfig);
                FileStream afs = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter asw = new StreamWriter(afs);
                asw.Write(yaml);
                asw.Close();
                Log.LogOut("", "行为控制:参数初始化执行成功");
            }
        }

        /// <summary>
        /// 对机器人的受邀加入群的处理方法
        /// </summary>
        /// <param name="session">SESSION</param>
        /// <param name="e">受邀入群事件</param>
        /// <returns>消息阻隔情况</returns>
        public async Task<bool> BotInvitedJoinGroup(MiraiHttpSession session, IBotInvitedJoinGroupEventArgs e)
        {
            bool flag = GroupRequestChecker();
            if (flag)
            {
                await session.HandleBotInvitedJoinGroupAsync(e, GroupApplyActions.Allow);
            }
            else
            {
                await session.HandleBotInvitedJoinGroupAsync(e, GroupApplyActions.Deny, "根据相关的设置，还不能将机器人拉入群内哦！QAQ");
            }
            Log.LogOut("", "行为控制:来自群 " + e.FromGroup + " 的 " + e.NickName + "，QQ为 " + e.FromQQ + "，发送了拉群请求给机器人，处理情况：" + flag);
            return false;
        }

        /// <summary>
        /// 对机器人受到好友申请的处理方法
        /// </summary>
        /// <param name="session">SESSION</param>
        /// <param name="e">受到好友申请事件</param>
        /// <returns>消息阻隔情况</returns>
        public async Task<bool> NewFriendApply(MiraiHttpSession session, INewFriendApplyEventArgs e)
        {
            bool flag = FriendRequestChecker();
            if (flag)
            {
                await session.HandleNewFriendApplyAsync(e, FriendApplyAction.Allow);
            }
            else
            {
                await session.HandleNewFriendApplyAsync(e, FriendApplyAction.Deny, "根据相关的设置，还不能添加机器人为好友哦！QAQ");
            }
            Log.LogOut("", "行为控制:来自群 " + e.FromGroup + " 的 " + e.NickName + "，QQ为 " + e.FromQQ + "，发送了添加好友请求给机器人，处理情况：" + flag);
            return false;
        }

        /// <summary>
        /// 检查组的行为参数信息
        /// </summary>
        /// <returns>消息阻隔情况</returns>
        private bool GroupRequestChecker()
        {
            Log.LogOut("", "行为控制:收到组邀请检查请求");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string s = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/config/BotBehaviourConfig.yaml");
            var c = deserializer.Deserialize<BotBehaviourConfig>(s);
            if (c.request.groupRequest.ToLower().Equals("t"))
            {
                return true;
            }
            else if (c.request.groupRequest.ToLower().Equals("f"))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查好友允许行为参数信息
        /// </summary>
        /// <returns>消息阻隔情况</returns>
        private bool FriendRequestChecker()
        {
            Log.LogOut("", "行为控制:收到好友邀请检查请求");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string s = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/config/BotBehaviourConfig.yaml");
            var c = deserializer.Deserialize<BotBehaviourConfig>(s);
            if (c.request.friendRequest.ToLower().Equals("t"))
            {
                return true;
            }
            else if (c.request.friendRequest.ToLower().Equals("f"))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 指令菜单控制
        /// </summary>
        /// <param name="session">SESSION</param>
        /// <param name="e">群消息事件</param>
        /// <returns>消息阻隔情况</returns>
        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
        {
            string str = string.Join(null, (IEnumerable<IMessageBase>)e.Chain); // 取消息
            string[] strArray = str.Split(new char[2] { '[', ']' }); // 分割Mirai码部分
            str = strArray[2];
            if (e.Sender.Id != _botqq)
            {
                if (str.Equals(".help"))
                {
                    BotBehaviourConfigMenuTrans();
                    IMessageBase plainMenuHelp = new PlainMessage(this._help);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, plainMenuHelp);
                }
            }
            return false;
        }

        /// <summary>
        /// 行为菜单解析
        /// </summary>
        private void BotBehaviourConfigMenuTrans()
        {
            Log.LogOut("", "行为控制:菜单检查事件被触发");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string s = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/config/BotBehaviourConfig.yaml");
            var c = deserializer.Deserialize<BotBehaviourConfig>(s);
            this._help = c.menu.help;
        }
    }

    /// <summary>
    /// 对机器人行为的序列化所需要的实体类
    /// </summary>
    public class BotBehaviourConfig
    {
        public Request request { get; set; }
        public Menu menu { get; set; }

    }

    public class Request
    {
        public string friendRequest { get; set; }
        public string groupRequest { get; set; }
    }

    public class Menu
    {
        public string help { get; set; }
    }
}
