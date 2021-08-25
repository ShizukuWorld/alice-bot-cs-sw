using System;
using System.Collections.Generic;
using System.IO;
using alice_bot_cs_sw.Core;
using alice_bot_cs_sw.Tools;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace alice_bot_cs_sw.Extensions
{
    public class OsuApiV1Helper
    {
        string _apikey = ""; // osu v1 api

        /// <summary>
        /// 构造方法
        /// </summary>
        public OsuApiV1Helper()
        {
            CreateOsuApiV1HelperConfig();
            ReadOsuApiV1HelperConfig();
        }

        /// <summary>
        /// 创建config
        /// </summary>
        private static void CreateOsuApiV1HelperConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + @"/config/";
            string configFilePath = configPath + @"OsuApiV1HelperConfig.yaml";

            if (false == System.IO.File.Exists(configFilePath))
            {
                FileStream fs = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();

                var config = new OsuApiV1HelperConfig
                {
                    apikey = "",
                };
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                var yaml = serializer.Serialize(config);

                FileStream fsc = new FileStream(configFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter swc = new StreamWriter(fsc);
                swc.Write(yaml);
                swc.Close();

                Log.LogOut("", "OSU查询:执行参数初始化成功");
            }
        }

        private void ReadOsuApiV1HelperConfig()
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            string config = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/config/OsuApiV1HelperConfig.yaml");
            var sb = deserializer.Deserialize<OsuApiV1HelperConfig>(config);

            _apikey = sb.apikey;

            Log.LogOut("", "OSU查询:读取API(V1)已完成");
        }

        /// <summary>
        /// 查询Best Perfermance
        /// </summary>
        /// <param name="username">需要查询的用户名称</param>
        /// <returns>需要发送的信息</returns>
        public string OsuGetUserBP(string username)
        {
            string userBP = null;

            return userBP;
        }

        public string OsuGetUserInfo(string username)
        {
            string userInfo = null;
            string data = HttpTool.Get($"https://osu.ppy.sh/api/get_user?k={_apikey}&u={username}","");

            OsuGetUserInfoJson result = JsonConvert.DeserializeObject<OsuGetUserInfoJson>(data);
            if(result.osuGetUserInfoJsonData.user_id.Length > 0)
            {
                userInfo = $"已获取到用户{username}信息:\n" +
                $"用户id:{result.osuGetUserInfoJsonData.user_id}\n" +
                $"用户加入日期:{result.osuGetUserInfoJsonData.join_date}\n" +
                $"排位得分RS:{result.osuGetUserInfoJsonData.ranked_score}\n" +
                $""; // todo:没有写完的回答，还需要添加数据库查找功能@mashirosa
            }
            
            return userInfo;
        }
    }

    public class OsuApiV1HelperConfig
    {
        public string apikey { get; set; }
    }

    public class OsuGetUserInfoJson
    {
        public OsuGetUserInfoJsonData osuGetUserInfoJsonData { get; set; }
    }

    public class OsuGetUserInfoJsonData
    {
        public string user_id { get; set; }
        public string username { get; set; }
        public string join_date { get; set; }
        public string count300 { get; set; }
        public string count100 { get; set; }
        public string count50 { get; set; }
        public string playcount { get; set; }
        public string ranked_score { get; set; }
        public string total_score { get; set; }
        public string pp_rank { get; set; }
        public string level { get; set; }
        public string pp_raw { get; set; }
        public string accuracy { get; set; }
        public string count_rank_ss { get; set; }
        public string count_rank_ssh { get; set; }
        public string count_rank_s { get; set; }
        public string count_rank_sh { get; set; }
        public string count_rank_a { get; set; }
        public string country { get; set; }
        public string total_seconds_played { get; set; }
        public string pp_country_rank { get; set; }
        public List<string> events { get; set; }
    }
}
