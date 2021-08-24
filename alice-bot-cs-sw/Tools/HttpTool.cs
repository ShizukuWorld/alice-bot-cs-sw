using System.IO;
using System.Net;
using System.Text;

namespace alice_bot_cs_sw.Tools
{
    public sealed class HttpTool
    {
        /// <summary>
        /// 工具类构造方法。
        /// </summary>
        public HttpTool()
        {
        }

        /// <summary>
        /// Http Get方法
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postDataStr">信息</param>
        /// <returns>Get内容</returns>
        public static string Get(string url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// Http流式信息读取字节类。
        /// </summary>
        /// <param name="url">读取地址</param>
        /// <returns>信息</returns>
        public static byte[] GetBytesFromUrl(string url) // 从Url读取流式数据
        {
            byte[] b = null;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();
            using (Stream stream = myResp.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                int count = 0;
                do
                {
                    byte[] buf = new byte[1024];
                    count = stream.Read(buf, 0, 1024);
                    ms.Write(buf, 0, count);
                } while (stream.CanRead && count > 0);
                b = ms.ToArray();
            }
            return b;
        }

        /// <summary>
        /// Http流式信息写入存储类。
        /// </summary>
        /// <param name="fileName">目标文件名</param>
        /// <param name="path">目标文件位置</param>
        /// <param name="content">写入流内容</param>
        public static void WriteBytesToFile(string fileName, string path, byte[] content)
        {
            if (false == System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            try
            {
                w.Write(content);
            }
            finally
            {
                fs.Close();
                w.Close();
            }
        }
    }
}
