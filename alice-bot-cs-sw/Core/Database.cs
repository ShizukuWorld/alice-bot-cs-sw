using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace alice_bot_cs_sw.Core
{
    /// <summary>
    /// 数据库操作类。
    /// </summary>
    public class Database
    {
        //public static string db = AppDomain.CurrentDomain.BaseDirectory + @"/database/Bot.db";
        public static string DbPath = AppDomain.CurrentDomain.BaseDirectory + @"/database/";
        public static string DbFilePath = DbPath + @"Bot.db";
        public static SQLiteConnection SqliteConnection = new SQLiteConnection("data source=" + DbFilePath);

        /// <summary>
        /// 构造方法。
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// 创建一个新的机器人使用的SQLite数据库
        /// </summary>
        public static void CreateNewSqLiteDatabase()
        {
            if (false == System.IO.File.Exists(DbFilePath))
            {
                FileStream fs = new FileStream(DbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();

                if (Database.SqliteConnection.State != System.Data.ConnectionState.Open)
                {
                    Database.SqliteConnection.Open();
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.Connection = Database.SqliteConnection;

                    cmd.CommandText = "CREATE TABLE " + "qquser" +
                                      "(id int, qqnumber varchar, permission int, mcid varchar, osuid varchar, arcid bigint, sleeptime varchar)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE " + "qqgroup" +
                                      "(id int, qgnumber varchar, permission int, setuset int)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE " + "config" +
                                      "(id int, subject varchar, data varchar)";
                    cmd.ExecuteNonQuery();

                    Database.SqliteConnection.Close();
                }
            }
        }

        /// <summary>
        /// 在机器人SQLite的数据库中新增一个列。
        /// </summary>
        /// <param name="tableName">目标数据表名</param>
        /// <param name="columnName">列名</param>
        /// <param name="columnClass">列类型</param>
        public static void CreateNewSqLiteColumn(string tableName, string columnName, string columnClass)
        {
            if (false == System.IO.File.Exists(DbFilePath)) // 初始化鉴别
            {
                FileStream fs = new FileStream(DbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();

                if (Database.SqliteConnection.State != System.Data.ConnectionState.Open)
                {
                    Database.SqliteConnection.Open();
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.Connection = Database.SqliteConnection;

                    cmd.CommandText = "ALTER TABLE " + $"{tableName} " +
                                      "ADD COLUMN " + $"{columnName} " + $"{columnClass}";
                    cmd.ExecuteNonQuery();

                    Database.SqliteConnection.Close();
                }
            }
        }

        /// <summary>
        /// 在SQLite中User新增一个新的用户记录
        /// </summary>
        /// <param name="qqNumber">用户的QQ号</param>
        public static void CreateNewSqLiteUserInfo(long qqNumber)
        {
            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;

            int totalRow = 0;
            cmd.CommandText = "SELECT count(*) FROM qquser";
            SQLiteDataReader srTotal = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (srTotal.Read())
            {
                totalRow = srTotal.GetInt32(0);
            }
            else
            {
                totalRow += 1;
            }
            srTotal.Close();
            cmd.ExecuteNonQueryAsync();


            cmd.CommandText = $"SELECT * FROM qquser WHERE qqnumber IS {qqNumber};";


            SQLiteDataReader srDetail = cmd.ExecuteReader(CommandBehavior.SingleRow);

            if (srDetail.HasRows)
            {
                srDetail.Close();
            }
            else
            {
                srDetail.Close();
                Log.LogOut("", $"数据库:发现新用户:{qqNumber},正在注册到数据库");
                cmd.CommandText = "INSERT INTO " + "qquser" + " " +
                                  $"VALUES ('{totalRow}', '{qqNumber}', 1, null, null, null, null)";
                cmd.ExecuteNonQueryAsync();

            }

            SqliteConnection.Close();
        }

        /// <summary>
        /// 在SQLite中User新增一个新的QQ群记录
        /// </summary>
        /// <param name="qgNumber">QQ群的群号</param>
        public static void CreateNewSqLiteGroupInfo(long qgNumber)
        {
            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;

            int totalRow = 0;
            cmd.CommandText = "SELECT count(*) FROM qqgroup";
            SQLiteDataReader srTotal = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (srTotal.Read())
            {
                totalRow = srTotal.GetInt32(0);
            }
            else
            {
                totalRow += 1;
            }
            srTotal.Close();
            cmd.ExecuteNonQueryAsync();


            cmd.CommandText = $"SELECT * FROM qqgroup WHERE qgnumber IS {qgNumber};";


            SQLiteDataReader srDetail = cmd.ExecuteReader(CommandBehavior.SingleRow);

            if (srDetail.HasRows)
            {
                srDetail.Close();
            }
            else
            {
                srDetail.Close();
                Log.LogOut("", $"数据库:发现新群组:{qgNumber},正在注册到数据库");
                cmd.CommandText = "INSERT INTO " + "qqgroup" + " " +
                                  $"VALUES ('{totalRow}', '{qgNumber}', 1, 1)";
                cmd.ExecuteNonQueryAsync();

            }

            SqliteConnection.Close();
        }

        /// <summary>
        /// 查询某用户的权限组
        /// </summary>
        /// <param name="qqNumber">查询对象的QQ号</param>
        /// <returns>对应对象的权限组</returns>
        public static int CheckSqLiteUserPermission(long qqNumber)
        {
            int permission = 0;

            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;
            cmd.CommandText = $"SELECT permission FROM qquser WHERE qqnumber={qqNumber}";
            SQLiteDataReader sr = cmd.ExecuteReader(); // 读取结果集
            
            while (sr.Read())
            {
                permission = sr.GetInt32(0);
            }
            SqliteConnection.Close();
            return permission;
        }

        /// <summary>
        /// 查询某群的权限组
        /// </summary>
        /// <param name="qgNumber">查询对象群的群号</param>
        /// <returns>对应对象的权限组</returns>
        public static int CheckSqLiteGroupSetuset(long qgNumber)
        {
            int setuset = 0;

            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;
            cmd.CommandText = $"SELECT setuset FROM qqgroup WHERE qgnumber={qgNumber}";
            SQLiteDataReader sr = cmd.ExecuteReader(); // 读取结果集

            while (sr.Read())
            {
                setuset = sr.GetInt32(0);
            }
            SqliteConnection.Close();
            return setuset;
        }
        

        /// <summary>
        /// 直接执行一个数据库命令操作，且不返回数据
        /// </summary>
        /// <param name="qqNumber">操作人的qq</param>
        /// <param name="command"></param>
        public static void SqLiteCommandExecute(long qqNumber, string command)
        {
            if (CheckSqLiteUserPermission(qqNumber) == 3)
            {
                SqliteConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = SqliteConnection;

                Log.LogOut("", $"数据库:{qqNumber}正在通过远程执行数据库操作");
                cmd.CommandText = command;
                cmd.ExecuteNonQueryAsync();

                SqliteConnection.Close();
            }
        }

        /// <summary>
        /// 直接执行数据库的查询
        /// </summary>
        /// <param name="qqNumber">操作人的QQ</param>
        /// <param name="command">命令</param>
        /// <returns>查询结果</returns>
        public static string SqLiteCommandReader(long qqNumber, string command)
        {
            string result = null;
            if (CheckSqLiteUserPermission(qqNumber) == 3)
            {
                SqliteConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = SqliteConnection;

                Log.LogOut("", $"数据库:{qqNumber}正在通过远程执行数据库查询");
                cmd.CommandText = command;
                SQLiteDataReader sr = cmd.ExecuteReader(); // 读取结果集
                result = sr.ToString();
                cmd.ExecuteNonQueryAsync();
                
                SqliteConnection.Close();
            }

            return result;
        }
        
        /// <summary>
        /// 更改某个人或者某个群的权限
        /// </summary>
        /// <param name="qqNumber">操作人的QQ</param>
        /// <param name="type">是用户还是群聊</param>
        /// <param name="target">目标</param>
        /// <param name="permission">权限</param>
        public static void ChangeSqLiteQqQgPermission(long qqNumber, int type, string target, string permission)
        {
            SqliteConnection.Open();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;
            if (type == 1)
            {
                Log.LogOut("", $"数据库:{qqNumber}执行了对用户{target}的权限更改->{permission}");
                cmd.CommandText = $"UPDATE qquser SET permission = {permission} WHERE qqnumber = {target}";
            }else if (type == 2)
            {
                Log.LogOut("", $"数据库:{qqNumber}执行了对群聊{target}的权限更改->{permission}");
                cmd.CommandText = $"UPDATE qqgroup SET permission = {permission} WHERE qgnumber = {target}";
            }

            cmd.ExecuteNonQueryAsync();

            SqliteConnection.Close();
        }
        
        public static void ChangeSqLiteQgSetuSet(long qqNumber, string target, string setuset)
        {
            SqliteConnection.Open();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;

            Log.LogOut("", $"数据库:{qqNumber}执行了对群聊{target}的色图功能权限更改->{setuset}");
            cmd.CommandText = $"UPDATE qqgroup SET setuset = {setuset} WHERE qgnumber = {target}";

            cmd.ExecuteNonQueryAsync();

            SqliteConnection.Close();
        }
    }
}
