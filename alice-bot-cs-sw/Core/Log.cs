using System;
using System.IO;
using System.Text;

namespace alice_bot_cs_sw.Core
{
    public class Log
    {
        /// <summary>
        /// 实例化方法，使用private，该类不能被实例化。
        /// </summary>
        private Log()
        {
        }

        /// <summary>
        /// 输出日志。
        /// </summary>
        /// <param name="fileName">需要输出日志的日志文件名，建议保留为空</param>
        /// <param name="message">需要输出的日志消息</param>
        /// <returns>日志输出的情况，布尔值</returns>
        public static bool LogOut(string fileName, string message)
        {
            DateTime dt = DateTime.Now; // 设置日志时间 
            string time = dt.ToString("yyyy-MM-dd HH:mm:ss"); //年-月-日 时：分：秒 
            string logName = dt.ToString("yyyy-MM-dd"); //日志名称 
            string logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Path.Combine("log", fileName)); //日志存放路径 
            string log = Path.Combine(logPath, string.Format("{0}.log", logName)); //路径 + 名称
            try
            {
                FileInfo info = new FileInfo(log);
                if (info.Directory != null && !info.Directory.Exists)
                {
                    info.Directory.Create();
                }
                Console.WriteLine(time + ":" + message); // 向控制台输出信息
                using (StreamWriter write = new StreamWriter(log, true, Encoding.GetEncoding("utf-8"))) // 输出日志
                {
                    write.WriteLine(time + ":" + message);
                    write.Flush();
                    write.Close();
                    write.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(time + ":" + "日志记录发生错误:" + e.StackTrace);
                return false;
            }
        }
    }
}

