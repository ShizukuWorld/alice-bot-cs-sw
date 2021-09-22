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
        public static string dbPath = AppDomain.CurrentDomain.BaseDirectory + @"/database/";
        public static string dbFilePath = dbPath + @"Bot.db";
        public static SQLiteConnection SqliteConnection = new SQLiteConnection("data source=" + dbFilePath);

        /// <summary>
        /// 构造方法。
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// 创建一个新的机器人使用的SQLite数据库
        /// </summary>
        public static void CreateNewSQLiteDatabase()
        {
            if (false == System.IO.File.Exists(dbFilePath))
            {
                FileStream fs = new FileStream(dbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
        public static void CreateNewSQLiteColumn(string tableName, string columnName, string columnClass)
        {
            if (false == System.IO.File.Exists(dbFilePath)) // 初始化鉴别
            {
                FileStream fs = new FileStream(dbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
        /// <returns></returns>
        public static void CreateNewSQLiteUserInfo(long qqNumber)
        {
            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;

            int totalRow = 0;
            cmd.CommandText = "SELECT count(*) FROM qquser";
            SQLiteDataReader sr1 = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (sr1.Read())
            {
                totalRow = sr1.GetInt32(0);
            }
            else
            {
                totalRow += 1;
            }
            sr1.Close();
            cmd.ExecuteNonQueryAsync();


            cmd.CommandText = $"SELECT * FROM qquser WHERE qqnumber IS {qqNumber};";


            SQLiteDataReader sr2 = cmd.ExecuteReader(CommandBehavior.SingleRow);

            if (sr2.HasRows)
            {
                sr2.Close();
            }
            else
            {
                sr2.Close();
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
        /// <returns></returns>
        public static void CreateNewSQLiteGroupInfo(long qgNumber)
        {
            SqliteConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SqliteConnection;

            int totalRow = 0;
            cmd.CommandText = "SELECT count(*) FROM qqgroup";
            SQLiteDataReader sr1 = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (sr1.Read())
            {
                totalRow = sr1.GetInt32(0);
            }
            else
            {
                totalRow += 1;
            }
            sr1.Close();
            cmd.ExecuteNonQueryAsync();


            cmd.CommandText = $"SELECT * FROM qqgroup WHERE qgnumber IS {qgNumber};";


            SQLiteDataReader sr2 = cmd.ExecuteReader(CommandBehavior.SingleRow);

            if (sr2.HasRows)
            {
                sr2.Close();
            }
            else
            {
                sr2.Close();
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
        public static int CheckSQLiteUserPermission(long qqNumber)
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

        public static int CheckSQLiteGroupSetuset(long qgNumber)
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
    }
}
