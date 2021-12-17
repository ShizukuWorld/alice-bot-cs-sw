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
            if (str.Contains("色图") || str.Contains("setu")) await SetuPic(session, e, str);
            #endregion
            
            // osu信息查询
            #region OsuProfile
            if (str.Contains("osu查询 ") || str.Contains(".osupf ")) await OsuProfile(session, e, str);
            #endregion

            // 寻找随机猫猫
            #region RandomCat
            if (str.Equals("随机猫猫") || str.Equals(".cat")) await RandomCat(session, e, str);
            #endregion
            
            // 权限修改
            #region PermissionChanger
            if(str.Contains("!setuset")) await SetuSetChanger(session, e, e.Sender.Id, str);
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

        /// <summary>
        /// 群聊消息的logger
        /// </summary>
        /// <param name="e">群聊消息flag</param>
        private async Task MsgLogger(IGroupMessageEventArgs e)
        {
            //用户注册
            Database.CreateNewSqLiteUserInfo(e.Sender.Id);
            //群聊注册
            Database.CreateNewSqLiteGroupInfo(e.Sender.Group.Id);
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
        
        /// <summary>
        /// 获取色图方法
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="e">消息flag</param>
        /// <param name="str">群聊消息</param>
        /// <returns></returns>
        private async Task<bool> SetuPic(MiraiHttpSession session, IGroupMessageEventArgs e, string str)
        {
            string reply = ""; //回复准备
            IMessageBase plain = new PlainMessage(reply); // 构造消息器
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
                int setuset = Database.CheckSqLiteGroupSetuset(e.Sender.Group.Id);
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
            return false;
        }
        
        /// <summary>
        /// OSU成绩查询
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="e">消息flag</param>
        /// <param name="str">群聊消息</param>
        /// <returns></returns>
        private async Task<bool> OsuProfile(MiraiHttpSession session, IGroupMessageEventArgs e, string str)
        {

            str.Split(' ');
            string target = str.Substring((str.IndexOf(" ", StringComparison.Ordinal)), (str.Length - str.IndexOf(" ", StringComparison.Ordinal)));
            target = target.Replace(" ", "");
            
            if (target.Length > 0)
            {
                string reply = "正在查询中！\n若长时间未返回内容，则可能指定用户不存在..";
                IMessageBase plain = new PlainMessage(reply); 
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                OsuApiV1Helper osuApiV1Helper = new OsuApiV1Helper();

                reply = osuApiV1Helper.OsuGetUserInfo(target);
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                return false;

            }
            return false;
        }

        /// <summary>
        /// 随机猫猫图片
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="e">消息flag</param>
        /// <param name="str">群聊消息</param>
        /// <returns></returns>
        private async Task<bool> RandomCat(MiraiHttpSession session, IGroupMessageEventArgs e, string str)
        {
            string reply = "正在搜寻猫猫哦！\n若长时间未返回内容，则可能猫猫走丢了....";
            IMessageBase plain = new PlainMessage(reply);
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
        }
        
        /// <summary>
        /// 改变一个用户的或者是一个群的权限
        /// </summary>
        /// <param name="qqSender">消息发送者</param>
        /// <param name="str">消息内容</param>
        /// <returns>消息阻隔</returns>
        private async Task<bool> SetuSetChanger(MiraiHttpSession session, IGroupMessageEventArgs e, long qqSender, string str)
        {
            string type; // 需要更改的权限种类
            string target; // 目标
            string permission; // 权限

            if (Database.CheckSqLiteUserPermission(qqSender) == 3)
            {
                //分裂消息为数组
                str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", ",");
                string[] arr = str.Split(',');

                target = arr[1];
                permission = arr[2];
                
                if (target.Contains("this"))
                {
                    target = e.Sender.Group.Id.ToString();
                }
                
                Database.ChangeSqLiteQgSetuSet(qqSender, target, permission);
                string reply = "权限修改成功！";
                IMessageBase plain = new PlainMessage(reply); 
                IMessageBase atMsg = new AtMessage(qqSender);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain, atMsg);
                return false;
            }
            else
            {
                string reply = "你寄吧谁啊想改色图权限！！！";
                IMessageBase plain = new PlainMessage(reply);
                IMessageBase atMsg = new AtMessage(qqSender);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain, atMsg);
                return false;
            }
        }
    }
}
