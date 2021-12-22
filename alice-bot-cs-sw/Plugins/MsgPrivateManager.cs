using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using alice_bot_cs_sw.Core;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace alice_bot_cs_sw.Plugins
{
    public partial class MsgPrivateManager : IFriendMessage
    {
        private string _msg = null;
        private string _str = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MsgPrivateManager()
        {
        }

        /// <summary>
        /// 好友消息处理
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="e">消息事件flag</param>
        /// <returns>异步阻隔</returns>
        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e)
        {
            //消息处理
            #region MsgLogger
            await MsgLogger(e);
            string msg = _msg;
            string str = _str;
            #endregion

            //权限修改
            #region PermissionChanger
            if(str.Contains("!cp")) await PermissionChanger(e.Sender.Id, str, session, e);
            #endregion
            
            return false;
        }

        private async Task MsgLogger(IFriendMessageEventArgs e)
        {
            //用户注册
            Database.CreateNewSqLiteUserInfo(e.Sender.Id);
            //消息分割
            string strMirai = string.Join(null, (IEnumerable<IMessageBase>) e.Chain); // 取消息
            string[] strArray = strMirai.Split(new char[2] {'[', ']'}); // 分割Mirai码部分
            string strFinal = strArray[2];
            _str = strFinal;
            //Log记录
            string finalMsg = $"消息:来自私聊{e.Sender.Id}:{strMirai}";
            _msg = finalMsg;
            Log.LogOut("", finalMsg);
        }

        /// <summary>
        /// 改变一个用户的或者是一个群的权限
        /// </summary>
        /// <param name="qqSender">消息发送者</param>
        /// <param name="str">消息内容</param>
        /// <returns>消息阻隔</returns>
        private async Task<bool> PermissionChanger(long qqSender, string str, MiraiHttpSession session, IFriendMessageEventArgs e)
        {
            string type; // 需要更改的权限种类
            string target; // 目标
            string permission; // 权限

            if (Database.CheckSqLiteUserPermission(qqSender) == 3)
            {
                //分裂消息为数组
                str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", ",");
                string[] arr = str.Split(',');

                type = arr[1];
                target = arr[2];
                permission = arr[3];
                
                if (type.ToUpper().Contains("USER"))
                {
                    Database.ChangeSqLiteQqQgPermission(qqSender, 1, target, permission);
                    IMessageBase reply = new PlainMessage("对成员的权限修改已执行");
                    await session.SendFriendMessageAsync(e.Sender.Id, reply);
                }else if (type.ToUpper().Contains("GROUP"))
                {
                    Database.ChangeSqLiteQqQgPermission(qqSender, 2, target, permission);
                    IMessageBase reply = new PlainMessage("对群聊的权限修改已执行");
                    await session.SendFriendMessageAsync(e.Sender.Id, reply);
                }
            }
            return false;
        }

        private async Task<bool> SqlCommandExecutor(string qqSender, string command)
        {
            return false;
        }
    }
}