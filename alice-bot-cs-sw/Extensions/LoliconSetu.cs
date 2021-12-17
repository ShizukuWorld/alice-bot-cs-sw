using System;
using System.Collections.Generic;
using System.IO;
using alice_bot_cs_sw.Core;
using alice_bot_cs_sw.Tools;
using Newtonsoft.Json;

namespace alice_bot_cs_sw.Extensions
{
    public class LoliconSetu
    {
        string _apikey = "";
        int _pid = 0;
        int _p = 0;
        int _uid = 0;
        string _title = "";
        string _author = "";
        string _ext = "";
        string _originalUrl = "";
        string _regularUrl = "";
        bool _r18 = false;
        int _width = 0;
        int _height = 0;
        List<string> _tag;
        string _setuData = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data/RandomSetu/lolicon");
        string _setuFile;

        public LoliconSetu()
        {
        }

        /// <summary>
        /// 获取无tag无r18的色图。
        /// </summary>
        /// <returns>色图存放地址</returns>
        public string GetSetu()
        {
            string setuJson = HttpTool.Get($"https://api.lolicon.app/setu/v2/?size=regular&r18=0", "");
            ParseSetu(setuJson); // 分析色图。
            int flag = DownloadSetu(); // 下载色图。
            Log.LogOut("", $"色图功能:色图功能调用-N");
            if (flag == 1)
            {
                return null;
            }
            return _setuFile;
        }

        /// <summary>
        /// 获取无tag的r18色图
        /// </summary>
        /// <returns>色图存放地址</returns>
        public string GetSetuR18()
        {
            string setuJson = HttpTool.Get($"https://api.lolicon.app/setu/v2/?size=regular&r18=1", "");
            ParseSetu(setuJson); // 分析色图。
            int flag = DownloadSetu(); // 下载色图。
            Log.LogOut("", $"色图功能:色图功能调用-R18");
            if(flag == 1)
            {
                return null;
            }
            return _setuFile;
        }

        public string GetSetuTag(string tagName)
        {
            string setuJson = HttpTool.Get($"https://api.lolicon.app/setu/v2/?size=regular&r18=0&tag={tagName}", "");
            ParseSetu(setuJson); // 分析色图。
            int flag = DownloadSetu(); // 下载色图。
            Log.LogOut("", $"色图功能:色图功能调用-Tag");
            if (flag == 1 || _pid == 0)
            {
                return null;
            }
            return _setuFile;
        }

        /// <summary>
        /// 处理返回的json信息。
        /// </summary>
        /// <param name="setuJson">需要反序列化的json</param>
        private int ParseSetu(string setuJson)
        {
            LoliconJson result = JsonConvert.DeserializeObject<LoliconJson>(setuJson);
            _regularUrl = result.data[0].urls.regular;
            _pid = result.data[0].pid;
            _ext = result.data[0].ext;
            
            return 0;
        }

        /// <summary>
        /// 下载色图。
        /// </summary>
        /// <returns>该方法的执行情况</returns>
        private int DownloadSetu()
        {
            if (_ext.Contains("jpg"))
            {
                _setuFile = Path.Combine(_setuData, _pid + ".jpg");
            }
            else if (_ext.Contains("png"))
            {
                _setuFile = Path.Combine(_setuData, _pid + ".png");
            }
            else if (_ext.Contains("jpeg"))
            {
                _setuFile = Path.Combine(_setuData, _pid + ".jpeg"); // 感觉这样不太好？
            }
            else
            {
                return 1;
            }

            byte[] pic = HttpTool.GetBytesFromUrl(this._regularUrl);
            HttpTool.WriteBytesToFile(this._setuFile, this._setuData, pic);
            return 0;
        }
    }

    /// <summary>
    /// 对Lolicon的json的解析反序列化使用的实体类。V2接口。
    /// </summary>
    public class LoliconJson
    {
        public string error { get; set; }
        public List<SetuImageJson> data { get; set; }
    }

    /// <summary>
    /// 对Lolicon的json的图片信息反序列化使用的实体类。V2接口。
    /// </summary>
    public class SetuImageJson
    {
        public int pid { get; set; }
        public int p { get; set; }
        public int uid { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public Urls urls { get; set; }
        public bool r18 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<string> tags { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
    }

    /// <summary>
    /// 对Lolicon的色图的获取反序列化的实体类。V2接口。
    /// </summary>
    public class Urls
    {
        public string regular { get; set; }
        //public string original { get; set; }
    }
}
