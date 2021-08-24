using System;
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
    }
}
