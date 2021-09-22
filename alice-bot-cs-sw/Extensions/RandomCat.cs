using System;
using System.IO;
using alice_bot_cs_sw.Tools;
using Newtonsoft.Json;

namespace alice_bot_cs_sw.Extensions
{
    public class RandomCat
    {
        private string _url;
        private string _catData = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data/RandomCat");
        private string _catFile;

        public RandomCat()
        {
        }

        /// <summary>
        /// 获取一只随机的可爱猫猫
        /// </summary>
        /// <returns>猫猫图的路径</returns>
        public string GetCat() 
        {
            string catJson = HttpTool.Get($"http://aws.random.cat/meow", "");

            CatJson result = JsonConvert.DeserializeObject<CatJson>(catJson);
            if (result.file.Length > 0)
            {
                _url = result.file;
            }

            return DownloadCatPic();
            
        }

        /// <summary>
        /// 下载指定的猫猫图
        /// </summary>
        /// <returns>猫猫图的路径</returns>
        private string DownloadCatPic()
        {
            Random ran = new Random();
            string id = FileTool.GetRandomString(8);
            string path = null;

            if (_url.Contains("jpg"))
            {
                _catFile = Path.Combine(_catData, id + ".jpg");
            }
            else if (_url.Contains("png"))
            {
                _catFile = Path.Combine(_catData, id + ".png");
            }
            else if(_url.Contains("gif"))
            {
                _catFile = Path.Combine(_catData, id + ".gif");
            }
            else
            {
                return "";
            }

            byte[] pic = HttpTool.GetBytesFromUrl(this._url);
            HttpTool.WriteBytesToFile(_catFile, _catData, pic);
            path = _catFile;
            return path;
        }
    }

    public class CatJson
    {
        public string file { get; set; }
    }
}
