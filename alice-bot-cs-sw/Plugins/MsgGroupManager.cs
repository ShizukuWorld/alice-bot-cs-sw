using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            //消息分割
            string str = string.Join(null, (IEnumerable<IMessageBase>)e.Chain); // 取消息
            string[] strArray = str.Split(new char[2] { '[', ']' }); // 分割Mirai码部分
            str = strArray[2];

            //回复准备
            string reply = "";
            IMessageBase plain = new PlainMessage(reply);

            //Log记录
            string msg = $"消息:来自群{e.Sender.Group.Id}:{str}";
            Log.LogOut("", msg);

            //色图插件
            if (str.Equals("色图来"))
            {
                reply = "正在获取色图！";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                LoliconSetu loliconSetu = new LoliconSetu();
                var path = loliconSetu.GetSetu();

                reply = "已获取到色图！正在发送～";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                await SendPictureAsync(session, path, e.Sender.Group.Id);

            }

            if (str.Equals("超级无敌色图"))
            {
                reply = "正在获取色图！";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);

                LoliconSetu loliconSetu = new LoliconSetu();
                var path = loliconSetu.GetSetuR18();

                reply = "已获取到色图！正在发送～";
                plain = new PlainMessage(reply);
                await session.SendGroupMessageAsync(e.Sender.Group.Id, plain);
                await SendPictureAsync(session, path, e.Sender.Group.Id);

            }

            if (str.Equals("龙图来"))
            {
                RandomLadyDragon randomLadyDragon = new RandomLadyDragon();
                var path = randomLadyDragon.GetRandomLadyDragonPic();
                await SendPictureAsync(session, path, e.Sender.Group.Id);
            }

            return false; // 消息阻隔
        }

        /// <summary>
        /// 发送图片方法。
        /// </summary>
        /// <param name="session">session</param>
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
