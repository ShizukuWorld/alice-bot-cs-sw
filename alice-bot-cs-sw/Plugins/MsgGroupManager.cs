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
        private string _msg = null;
        private string _str = null;
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
            //基础服务
            #region BasicService
            await MsgLogger(e); // 先执行消息的解析
            string reply = ""; //回复准备
            IMessageBase plain = new PlainMessage(reply); // 构造消息器
            string str = _str;
            #endregion
            
            //色图
            #region SetuPic
            await SetuPic(session, e, str);
            #endregion
            
            #region OsuProfile
            // osu信息查询
            if (str.Contains("osu查询 ") || str.Contains(".osupf "))
            {
                str.Split(' ');
                string target = str.Substring((str.IndexOf(" ", StringComparison.Ordinal)), (str.Length - str.IndexOf(" ", StringComparison.Ordinal)));
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
                    return false;
                }
            }
            #endregion

            #region RandomCat
            if (str.Equals("随机猫猫") || str.Equals(".cat"))
            {
                reply = "正在搜寻猫猫哦！\n若长时间未返回内容，则可能猫猫走丢了....";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                RandomCat randomCat = new RandomCat();
                string path = randomCat.GetCat();

                if (path.Length == 0)
                {
                    reply = "寻找猫猫途中发生了未知错误！";
                    plain = new PlainMessage(reply);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                    return false;
                }

                reply = "已获取到猫猫！正在尝试推送中！\n该功能因为原服务器太远而经常丢失...";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                await SendPictureAsync(session, path, e.Sender.Group.Id);
                return false;
                
            // 寻找随机猫猫
            }
            #endregion
            
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

        private async Task MsgLogger(IGroupMessageEventArgs e)
        {
            //用户注册
            Database.CreateNewSQLiteUserInfo(e.Sender.Id);
            //群聊注册
            Database.CreateNewSQLiteGroupInfo(e.Sender.Group.Id);
            //消息分割
            string strMirai = string.Join(null, (IEnumerable<IMessageBase>)e.Chain); // 取消息
            string[] strArray = strMirai.Split(new char[2] { '[', ']' }); // 分割Mirai码部分
            string strFinal = strArray[2];
            _str = strFinal;
            //Log记录
            string finalMsg = $"消息:来自群{e.Sender.Group.Id}:{strMirai}";
            _msg = finalMsg;
            Log.LogOut("", finalMsg);
        }
        
        private async Task<bool> SetuPic(MiraiHttpSession session, IGroupMessageEventArgs e, string str)
        {
            string reply = ""; //回复准备
            IMessageBase plain = new PlainMessage(reply); // 构造消息器
            if (str.Contains("色图") || str.Contains("setu"))
            {
                if (str.Contains("找色图 ") || str.Contains(".findsetu "))
                {
                    str.Split(' ');
                    string tag = str.Substring((str.IndexOf(" ", StringComparison.Ordinal)), (str.Length - str.IndexOf(" ", StringComparison.Ordinal)));
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
                        return false;
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
            return false;
        }
    }
}
