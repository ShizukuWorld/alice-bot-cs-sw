using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using alice_bot_cs_sw.Core;
using alice_bot_cs_sw.Extensions;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace alice_bot_cs_sw.Plugins
{
    public partial class MsgGroupManager : IGroupMessage
    {
        /// <summary>
        /// 用于群聊消息处理的实例化方法，保持为空。
        /// </summary>
        public MsgGroupManager()
        {
        }

        /// <summary>
        /// 群聊消息。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="e"></param>
        /// <returns>消息阻隔情况，默认为否</returns>
        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
        {
            //用户注册
            Database.CreateNewSQLiteUserInfo(e.Sender.Id);

            //群聊注册
            Database.CreateNewSQLiteGroupInfo(e.Sender.Group.Id);

            //消息分割
            string strMirai = string.Join(null, (IEnumerable<IMessageBase>)e.Chain); // 取消息
            string[] strArray = strMirai.Split(new char[2] { '[', ']' }); // 分割Mirai码部分
            string str = strArray[2];

            //回复准备
            string reply = "";
            IMessageBase plain = new PlainMessage(reply);

            //Log记录
            string msg = $"消息:来自群{e.Sender.Group.Id}:{strMirai}";
            Log.LogOut("", msg);

            //色图扩展
            if (str.Contains("色图") || str.Contains("setu"))
            {
                if (str.Contains("找色图 ") || str.Contains(".findsetu "))
                {
                    str.Split(' ');
                    string tag = str.Substring((str.IndexOf(" ")), (str.Length - str.IndexOf(" ")));
                    tag = tag.Replace(" ", "");
                    if(tag.Length > 0)
                    {
                        reply = "正在搜寻指定色图！\n若长时间未返回内容，则可能指定关键词不存在..";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                        LoliconSetu loliconSetu = new LoliconSetu();
                        string path = loliconSetu.GetSetuTag(tag);
                        if (path.Length == 0)
                        {
                            reply = "色图插件出现解析问题，本次请求被取消";
                            plain = new PlainMessage(reply);
                            await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                            return false;
                        }

                        reply = "已获取到色图！正在发送..";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                        await SendPictureAsync(session, path, e.Sender.Group.Id);
                    }
                }
                if (str.Equals("色图来") || str.Equals("随机色图") || str.Equals(".setu"))
                {
                    int setuset = Database.CheckSQLiteGroupSetuset(e.Sender.Group.Id);
                    if(setuset == 1)
                    {
                        reply = "正在获取色图！";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                        LoliconSetu loliconSetu = new LoliconSetu();
                        var path = loliconSetu.GetSetu();
                        if (path.Length == 0)
                        {
                            reply = "色图插件出现解析问题，本次请求被取消";
                            plain = new PlainMessage(reply);
                            await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                            return false;
                        }

                        reply = "已获取到色图！正在发送..";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                        await SendPictureAsync(session, path, e.Sender.Group.Id);
                        return false;
                    }
                    else if(setuset == 2)
                    {
                        reply = "正在获取色图！";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                        LoliconSetu loliconSetu = new LoliconSetu();
                        var path = loliconSetu.GetSetuR18();
                        if (path.Length == 0)
                        {
                            reply = "色图插件出现解析问题，本次请求被取消";
                            plain = new PlainMessage(reply);
                            await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                            return false;
                        }

                        reply = "已获取到色图！正在发送..";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                        await SendPictureAsync(session, path, e.Sender.Group.Id);
                        return false;
                    }
                    else
                    {
                        reply = "数据库中本群权限组不正确";
                        plain = new PlainMessage(reply);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                        return false;
                    }
                }
            }

            // osu信息查询
            if (str.Contains("osu查询 ") || str.Contains(".osupf "))
            {
                str.Split(' ');
                string target = str.Substring((str.IndexOf(" ")), (str.Length - str.IndexOf(" ")));
                target = target.Replace(" ", "");
                if (target.Length > 0)
                {
                    reply = "正在查询中！\n若长时间未返回内容，则可能指定用户不存在..";
                    plain = new PlainMessage(reply);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                    OsuApiV1Helper osuApiV1Helper = new OsuApiV1Helper();

                    reply = osuApiV1Helper.OsuGetUserInfo(target);
                    plain = new PlainMessage(reply);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                }
            }

            return false; // 消息阻隔
        }

        /// <summary>
        /// 发送图片方法。
        /// </summary>
        /// <param name="session">SESSION</param>
        /// <param name="path">路径</param>
        /// <param name="target">目标</param>
        /// <returns></returns>
        private async Task SendPictureAsync(MiraiHttpSession session, string path, long target)
        {
            ImageMessage msg = await session.UploadPictureAsync(UploadTarget.Group, path);
            IMessageBase[] chain = new IMessageBase[] { msg };
            await session.SendGroupMessageAsync(target, chain);
        }
    }
}
