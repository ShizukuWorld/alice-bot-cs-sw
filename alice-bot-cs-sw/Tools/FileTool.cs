using System;
using System.Security.Cryptography;
using System.Text;

namespace alice_bot_cs_sw.Tools
{
    public class FileTool
    {
        public FileTool()
        {
        }

        /// <summary>
        /// 获取文件夹中的文件数量
        /// </summary>
        /// <param name="srcPath">目标文件的文件夹</param>
        /// <returns>文件数量</returns>
        public static int GetFileNum(string srcPath)
        {
            int fileNum = 0;
            string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);

            foreach (string file in fileList)
            {
                if (System.IO.Directory.Exists(file))
                    GetFileNum(file);
                else
                    fileNum++;
            }

            return fileNum;
        }

        /// <summary>
        /// 随机字符串生成类
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>结果</returns>
        public static string GetRandomString(int length)
        {
            return RNGCryptoRandomService.GetRandomString(length);
        }
    }

    /// <summary>
    /// 强随机字符类 RNGCryptoRandomService
    /// </summary>
    public sealed class RNGCryptoRandomService
    {
        private static RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        /// <summary>
        /// 生成一个指定长度的随机字符串
        /// </summary>
        /// <param name="stringlength">字符串长度</param>
        /// <returns>随机字符串结果</returns>
        public static string GetRandomString(int stringlength)
        {
            return GetRandomString(null, stringlength);
        }

        //获得长度为stringLength的随机字符串，以key为字母表
        public static string GetRandomString(string key, int stringLength)
        {
            if (key == null || key.Length < 8)
            {
                key = "abcdefghijklmnopqrstuvwxyz1234567890";
            }

            int length = key.Length;
            StringBuilder randomString = new StringBuilder(length);
            for (int i = 0; i < stringLength; ++i)
            {
                randomString.Append(key[SetRandomSeeds(length)]);
            }

            return randomString.ToString();
        }

        /// <summary>
        /// 随机种子生成
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>随机种子结果</returns>
        private static int SetRandomSeeds(int length)
        {
            decimal maxValue = (decimal)long.MaxValue;
            byte[] array = new byte[8];
            _random.GetBytes(array);

            return (int)(Math.Abs(BitConverter.ToInt64(array, 0)) / maxValue * length);
        }
    }
}
