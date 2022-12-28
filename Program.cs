using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MiniExcelLibs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using testorm.Entity;
using System.IO;
using Microsoft.Spark.Sql.Types;

namespace testorm
{
    /*public class program
    {
        public static void Main(string[] args)
        {
            
        }
    }*/
    #region 代码生成
    public class program
    {
        public static void Main(string[] args)
        {
            // 要生成的表
            string tableName = "BI_CUSTOMER_FIELD";
            // 要跳过的字段
            string[] skipArr = new string[] { "CREATEDATE", "CREATEUSERID", "CREATEUSERNAME", "MODIFYDATE", "MODIFYUSERID", "MODIFYUSERNAME", "REMARK", "ENABLED" };
            // 版本号
            string version = "1.0";
            // 文件生成地址(默认不传就是桌面)
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/generate";
            // 要显示的作者
            string author = "GPF";
            // 需要显示的名称空间
            string nameSpaceStr = "Generate";
            //锦溪正式Oracle
            var oracleConn1 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=jxbaizedb-scan1.luxshare.com.cn)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedb)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // 1，连接数据库
            SqlSugarClient client = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = oracleConn1,
                DbType = SqlSugar.DbType.Oracle,
                IsAutoCloseConnection = true
            });

            // 2,查询表字段
            string sql = $@"SELECT
                              b.TABLE_NAME tableName                    --表名
	                        , c.COLUMN_NAME columnName                  --列名
	                        , nvl(f.COMMENTS, c.COLUMN_NAME) columnComment-- 列注释
	                        , c.DATA_TYPE columnType                    --字段类型 varchar之类的
                        FROM  USER_USERS a
                        LEFT JOIN all_tables b ON a.USERNAME = b.OWNER
                        LEFT JOIN user_tab_columns c ON b.TABLE_NAME = c.TABLE_NAME
                        LEFT JOIN all_col_comments f ON a.USERNAME = f.OWNER AND b.TABLE_NAME = f.TABLE_NAME AND c.COLUMN_NAME = f.COLUMN_NAME
                        WHERE b.TABLE_NAME = '{tableName}'";
            var dt = client.Ado.GetDataTable(sql);

            // 3、开始生成实体
            string entityStr = File.ReadAllText("Template/Entity.txt");
            entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
            entityStr = entityStr.Replace("{description}", " This is the class description");
            entityStr = entityStr.Replace("{auther}", author);
            entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
            entityStr = entityStr.Replace("{tableName}", tableName);
            entityStr = entityStr.Replace("{version}", version);
            entityStr = entityStr.Replace("{className}", ToPascal(tableName));
            // 开始填充实体类字段
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"]??"".ToString().ToUpper()))
                    continue;
                sb.Append('\n');
                sb.Append('	');
                sb.Append($"///<summary>");
                sb.Append('\n');
                sb.Append('	');
                sb.Append($"///{dt.Rows[i][2].ToString()}");
                sb.Append('\n');
                sb.Append('	');
                sb.Append($"///</summary>");

                sb.Append('\n');
                sb.Append('	');
                sb.Append("public ");
                sb.Append(checkType(dt.Rows[i][3].ToString()));
                sb.Append(' ');
                sb.Append(dt.Rows[i][1].ToString());
                sb.Append(' ');
                sb.Append(" { set; get;} ");
            }
            entityStr = entityStr.Replace("{content}", sb.ToString());

            var entityPath = filePath+"/"+ToPascal(tableName)+".cs";
            /*if (!File.Exists(entityPath))
            {
                File.Create(entityPath);
            }*/
            File.WriteAllText(entityPath, entityStr.ToString());
            // 4、开始生成Controller
            entityStr = File.ReadAllText("Template/Controller.txt");
            entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
            entityStr = entityStr.Replace("{description}", " This is the class description");
            entityStr = entityStr.Replace("{auther}", author);
            entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
            entityStr = entityStr.Replace("{version}", version);
            entityStr = entityStr.Replace("{className}", ToPascal(tableName));
            var controllerPath = filePath + "/" + ToPascal(tableName) + "Controller.cs";
            if (!File.Exists(entityPath))
            {
                File.Create(entityPath);
            }
            File.WriteAllText(controllerPath, entityStr.ToString());
            // 5、开始生成 IService
            entityStr = File.ReadAllText("Template/IService.txt");
            entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
            entityStr = entityStr.Replace("{description}", " This is the class description");
            entityStr = entityStr.Replace("{auther}", author);
            entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
            entityStr = entityStr.Replace("{version}", version);
            entityStr = entityStr.Replace("{className}", ToPascal(tableName));
            var IServicePath = filePath + "/I" + ToPascal(tableName) + "Services.cs";
            if (!File.Exists(entityPath))
            {
                File.Create(entityPath);
            }
            File.WriteAllText(IServicePath, entityStr.ToString());
            // 6、开始生成 Service
            entityStr = File.ReadAllText("Template/Service.txt");
            entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
            entityStr = entityStr.Replace("{description}", " This is the class description");
            entityStr = entityStr.Replace("{auther}", author);
            entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
            entityStr = entityStr.Replace("{version}", version);
            entityStr = entityStr.Replace("{className}", ToPascal(tableName));
            var ServicePath = filePath + "/" + ToPascal(tableName) + "Services.cs";
            if (!File.Exists(entityPath))
            {
                File.Create(entityPath);
            }
            File.WriteAllText(ServicePath, entityStr.ToString());
        }

        private static string ToPascal(string str)
        {
            string[] split = str.Split(new char[] { '/', ' ', '_', '.' });
            string newStr = "";
            foreach (var item in split)
            {
                char[] chars = item.ToCharArray();
                chars[0] = char.ToUpper(chars[0]);
                for (int i = 1; i < chars.Length; i++)
                {
                    chars[i] = char.ToLower(chars[i]);
                }
                newStr += new string(chars);
            }
            return newStr;
        }

        private static string checkType(string? value)
        {
            switch (value?? "VARCHAR2".ToUpper())
            {
                case "VARCHAR2":
                case "VARCHAR":
                case "CHAR":
                case "CLOB":
                    return "string";
                case "DATE":
                case "TIME":
                case "TIMESTAMP":
                    return "Date";
                case "DECIMAL":
                case "DOUBLE PRECISION":
                case "NUMBER":
                    return "decimal";
                case "INTEGER":
                    return "int";
                case "LONG":
                    return "long";
                default:
                    return "string";
            }
        }
    }
    #endregion
    #region c# 连接 starRock
    /*public class program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            string dns = "Server=10.32.44.207;User ID=root;Password=;port=8000;Database=Demo";
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = dns,
                DbType = SqlSugar.DbType.MySqlConnector,
                IsAutoCloseConnection = true,
            });
            Console.WriteLine("创建链接");
            var dt2 = Db.Ado.GetDataTable("SELECT *  FROM test.doris_sink");
            Console.WriteLine("获取数据成功: 数量为" + dt2.Rows.Count);

        }
    }*/
    #endregion
    #region 连接sqllite
    /*public class program
    {
        public static void Main(string[] args)
        {
            string connect = "data source=C:\\Users\\Administrator\\AppData\\Local\\Packages\\cf906e41-910f-41ab-9cae-5a814460689d_9g7jfgbsde0p2\\LocalState\\sqlite.db";
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = connect,
                DbType = SqlSugar.DbType.Sqlite
            });
            var dt = sqlSugarClient.Ado.GetDataTable("PRAGMA table_info(schedulework)");
            Console.WriteLine(dt.Rows.Count);
        }
    }*/
    #endregion
    #region 调用控制台项目，并操作控制台黑窗口
    /*public class program
    {
        public static void Main(string[] args)
        {
            //实例化一个进程类
            Process cmd = new Process();

            //获得系统信息，使用的是 systeminfo.exe 这个控制台程序
            cmd.StartInfo.FileName = "cmd.exe";

            //将cmd的标准输入和输出全部重定向到.NET的程序里

            cmd.StartInfo.UseShellExecute = false; //此处必须为false否则引发异常

            cmd.StartInfo.RedirectStandardInput = true; //标准输入
            cmd.StartInfo.RedirectStandardOutput = true; //标准输出
            cmd.StartInfo.RedirectStandardError = true;

            //不显示命令行窗口界面
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

             cmd.Start(); //启动进程
            Console.WriteLine(readToEnd(cmd));

            cmd.StandardInput.Write("dotnet C:\\Users\\Administrator\\Desktop\\publish2\\testorm.dll\r\n");
            Console.WriteLine(readToEnd(cmd));

            cmd.StandardInput.Write("1.86\r\n");
            Console.WriteLine(readToEnd(cmd));

            cmd.StandardInput.Write("75\r\n");
            Console.WriteLine(readToEnd(cmd));

            cmd.StandardInput.Write("\r\n");
            Console.WriteLine(readToEnd(cmd));

            cmd.WaitForExit();//等待控制台程序执行完成
            cmd.Close();//关闭该进程
        }

        private static string readToEnd(Process cmd)
        {
            string cmdText = "";
            var charNum = cmd.StandardOutput.Peek();
            int tmp;
            while (true)
            {
                if (cmd.StandardInput.)
                {
                    Console.WriteLine("结束");
                }
                cmdText = string.Concat(cmdText, (char)cmd.StandardOutput.Read());
                int length = cmdText.Length;
            }
        }
    }*/
    #endregion
    #region 控制台输入
    /*class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入您的身高 单位(米) 例如1.68");
            double height = double.Parse(Console.ReadLine());
            Console.WriteLine("请输入您的体重 单位(kg) 例如60");
            int weight = int.Parse(Console.ReadLine());

            double exponent = weight / (height * height);

            Console.WriteLine("您的身高{0} 体重{0} BMI{0}", height, weight, exponent);


            if (exponent < 18.5)
            {
                Console.WriteLine("其中过轻");
            }
            else if (exponent >= 18.5 && exponent <= 24.9)
            {
                Console.WriteLine("正常范围");
            }
            else if (exponent >= 24.9 && exponent < 29.9)
            {
                Console.WriteLine("体重过重");
            }
            else if (exponent >= 29.9)
            {
                Console.WriteLine("肥胖");
            }
            Console.ReadLine();
        }
    }*/
    #endregion
    #region c sharp 链接 clickHouse
    /*public class program
    {
        public static void Main(string[] args)
        {
            string connect = "Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=10.32.44.215;Port=8123;User=default;Password=;SocketTimeout=600000;Database=datasets;";
            SqlSugarClient client = new SqlSugarClient(new ConnectionConfig
            {
                IsAutoCloseConnection = true,
                ConnectionString = connect,
                DbType = SqlSugar.DbType.ClickHouse
            });
            var dt = client.Ado.GetDataTable("select * from datasets.ontime limit 1,30");
            var json = JsonConvert.SerializeObject(dt);
            Console.WriteLine(dt.Rows.Count);
        }
    }*/
    #endregion
    #region 链接spark(失败)
    /*public class program
    {
        public static void Main(string[] args)
        {
            // Create Spark session
            SparkSession spark =
                SparkSession
                    .Builder()
                    .AppName("word_count_sample")
                    .GetOrCreate();

            // Create initial DataFrame
            string filePath = args[0];
            DataFrame dataFrame = spark.Read().Text(filePath);

            //Count words
            DataFrame words =
                dataFrame
                    .Select(Split(Col("value"), " ").Alias("words"))
                    .Select(Explode(Col("words")).Alias("word"))
                    .GroupBy("word")
                    .Count()
                    .OrderBy(Col("count").Desc());

            // Display results
            words.Show();

            // Stop Spark session
            spark.Stop();
        }
    }*/
    #endregion
    #region 链接spark1（失败）
    /*public class program
    {
        public static void Main(string[] args)
        {
            var sparkSample = SparkSession.Builder().Config("hive.metastore.uris", "thrift://10.32.44.213:9083").GetOrCreate();//.Master("local").AppName("Spark to Hive").Config("hive.metastore.uris", "thrift://10.32.44.213:9083").EnableHiveSupport().GetOrCreate();
            var df = sparkSample.Read().Json("people.json");
            df.Show();

            string connection_url = "jdbc:hive2://10.32.44.213:10003";
            string dbtable = "<database table to access>";
            string user = "admin";
            string password = "<Login user password>";

            var jdbcdf = sparkSample.Read()
                .Format("jdbc")
                .Option("driver", "org.apache.hive.jdbc.HiveDrive")
                .Option("url", connection_url)
                //.Option("dbtable", dbtable)
                .Option("user", user).Load();
                //.Option("password", password).Load();
            jdbcdf.Show(); // Displays the content of the SQL table as a DataFrame

        }
    }*/
    #endregion
    #region 链接hive ODBC
    /*public class program
    {
        public static void Main(string[] args)
        {
            string dns = "DSN=sparkM;UID=;PWD=";
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = dns,
                DbType = SqlSugar.DbType.Odbc,
                IsAutoCloseConnection = true,
            });
            Console.WriteLine("创建链接");
            // show tables
            var dt2 = Db.Ado.GetDataTable("select * from test.t1 limit 0 ,30 ");
            Console.WriteLine("获取数据成功: 数量为" + dt2.Rows.Count);

        }
    }*/
    #endregion
    #region sqlsugar 对象映射错误检测
    /*
    public class Program
    {
        public static void Main(string[] args)
        {
            //锦溪正式Oracle
            var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=jxbaizedb-scan1.luxshare.com.cn)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedb)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = oracleConn,
                DbType = SqlSugar.DbType.Oracle
            });

            var list = sqlSugarClient.Queryable<Report>().ToList();

            // 获取原始表数据
            sqlSugarClient.Close();
            sqlSugarClient.Dispose();
        }
    }*/
    #endregion
    #region Oracle to Oracle 转移数据
    /*public class Program
    {
        public static void Main(string[] args)
        {
            string tableName = "AUTO_TURN";
            //锦溪正式Oracle
            var oracleConn1 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=jxbaizedb-scan1.luxshare.com.cn)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedb)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            // 越南正式oracle
            var oracleConn2 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST =vnc1pb1-scan1.luxshare.com.cn)(PORT = 1521))(CONNECT_DATA=(SERVICE_NAME=baizedb))) ;Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            // 锦溪测试Oracle
            //var oracleConn2 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.32.44.54)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedev)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = oracleConn1,
                DbType = SqlSugar.DbType.Oracle
            });

            // 获取原始表数据  OFFSET 60 ROWS FETCH NEXT 30 ROWS ONLY
            var sql = $"select * from {tableName} ";
            var dt = sqlSugarClient.Ado.GetDataTable(sql);
            sqlSugarClient.Close();
            sqlSugarClient.Dispose();
            // 链接Oracle
            SqlSugarClient sqlSugarClient2 = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = oracleConn2,
                DbType = SqlSugar.DbType.Oracle
            });
            //int a = sqlSugarClient2.Ado.ExecuteCommand(tableCreat.ToString());

            sqlSugarClient2.Fastest<System.Data.DataTable>().AS(tableName).BulkCopy(dt);
            sqlSugarClient2.Close();
            sqlSugarClient2.Dispose();
        }
    }*/
    #endregion
    #region 数据库搬迁从mysql 到 oracle
    /*public class Program
    {
        public static void Main(string[] args)
        {
            // 
            string tableName = "auto_report";
            bool createFlag = false;
            bool dataFlag = true;

            var mysqlConn = "Server=10.32.44.78;User ID=report01;Password=Baize.2022;port=3306;Database=aj_report;CharSet=utf8;pooling=true;SslMode=None;";
            // 锦溪正式Oracle
            var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST =vnc1pb1-scan1.luxshare.com.cn)(PORT = 1521))(CONNECT_DATA=(SERVICE_NAME=baizedb))) ;Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            // 锦溪测试Oracle
            //var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.32.44.54)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedev)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = mysqlConn,
                DbType = SqlSugar.DbType.MySql
            });
            // 生成建表语句
            List<string> sqls = new();
            var tableSql = $@"SELECT  distinct TABLE_NAME tableName
                                       , COLUMN_NAME columnName
                                       , COLUMN_KEY  columnConstraint
                                       , COLUMN_COMMENT columnComment
                                       , DATA_TYPE columnType
                                       , CHARACTER_MAXIMUM_LENGTH columnLength
                                       , CHARACTER_OCTET_LENGTH columnOctetLength
                                       , NUMERIC_PRECISION	columnNumberPrecision
                                       , NUMERIC_SCALE  columnNumberScale
                                       , COLUMN_DEFAULT columnDefault
                                       , IS_NULLABLE columnIsNullable
                                FROM INFORMATION_SCHEMA.COLUMNS
                                where TABLE_NAME = '{tableName}'
                                and TABLE_SCHEMA ='aj_report'";
            List<tableInfo> list = sqlSugarClient.Ado.SqlQuery<tableInfo>(tableSql);
            Console.WriteLine(list.Count);
            if (list.Count == 0)
            {
                throw new Exception("表不存在！");
            }
            StringBuilder tableCreat = new StringBuilder("CREATE TABLE ");
            tableCreat.Append(list.First().tableName.ToUpper());
            tableCreat.Append(" (");
            foreach (var item in list)
            {
                tableCreat.Append('\n');
                tableCreat.Append("     ");
                tableCreat.Append('"');
                tableCreat.Append(item.columnName.ToUpper());
                tableCreat.Append('"');
                tableCreat.Append(" ");
                tableCreat.Append(getType(SqlSugar.DbType.MySql, SqlSugar.DbType.Oracle, item));
                tableCreat.Append(" ");
                tableCreat.Append(item.columnIsNullable == "YES" ? "" : "NOT NULL ");
                tableCreat.Append(" ");
                tableCreat.Append(item.columnDefault == null ? "" : $" DEFAULT {item.columnDefault} ");
                tableCreat.Append(" ,");
            }
            tableCreat.Append(getConstraint(list));
            tableCreat.Remove(tableCreat.Length - 1, 1);
            tableCreat.Append('\n');
            tableCreat.Append(')');
            sqls.Add(tableCreat.ToString());
            // 注释\

            foreach (var item in list)
            {
                tableCreat.Clear();
                tableCreat.Append("COMMENT ON COLUMN ");
                tableCreat.Append(item.tableName.ToUpper());
                tableCreat.Append('.');
                tableCreat.Append('"');
                tableCreat.Append(item.columnName.ToUpper());
                tableCreat.Append('"');
                tableCreat.Append(" is ");
                tableCreat.Append($" '{item.columnComment}' ");
                sqls.Add(tableCreat.ToString());
            }

            // 获取原始表数据
            var sql = $"select * from aj_report.{tableName}";
            var dt = sqlSugarClient.Ado.GetDataTable(sql);
            sqlSugarClient.Close();
            sqlSugarClient.Dispose();
            // 链接Oracle
            SqlSugarClient sqlSugarClient2 = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = oracleConn,
                DbType = SqlSugar.DbType.Oracle
            });
            // 执行建表语句
            if (createFlag)
            {
                Console.WriteLine("开始执行建表！");
                using (sqlSugarClient2.Ado.OpenAlways())
                { //开启长连接

                    foreach (var str in sqls)
                    {
                        Console.WriteLine(str);
                        Console.WriteLine("\n");
                        sqlSugarClient2.Ado.ExecuteCommand(str);
                    }

                }
            }
            //int a = sqlSugarClient2.Ado.ExecuteCommand(tableCreat.ToString());
            if (dataFlag)
                sqlSugarClient2.Fastest<System.Data.DataTable>().AS(tableName.ToUpper()).BulkCopy(dt);
            sqlSugarClient2.Close();
            sqlSugarClient2.Dispose();
        }

        private static string getConstraint(List<tableInfo> list)
        {
            StringBuilder res = new StringBuilder();
            IEnumerable<string> keys = list.Where(x => !string.IsNullOrEmpty(x.columnConstraint)).Select(x => x.columnConstraint).Distinct();
            foreach (var key in keys)
            {
                switch (key)
                {
                    case "PRI":
                        res.Append($"\nPRIMARY KEY (\"{list.Where(x => x.columnConstraint == key).First().columnName.ToUpper()}\") ,");
                        break;
                    case "UNI":
                        IEnumerable<string> names = list.Where(x => x.columnConstraint == key).Select(x => x.columnName);
                        if (names.Count() == 1)
                        {
                            res.Append($"\nCONSTRAINT {list.Where(x => x.columnConstraint == key).First().tableName.ToUpper()}_U1 UNIQUE (\"{names.First().ToUpper()}\"),");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string name in names)
                            {
                                sb.Append('"');
                                sb.Append(name);
                                sb.Append('"');
                                sb.Append(',');
                            }
                            res.Append($"CONSTRAINT constraint_name UNIQUE ({sb.Remove(sb.Length - 1, 1)})");
                        }
                        break;
                    default:
                        break;
                }
            }
            return res.ToString();
        }

        private static string getType(SqlSugar.DbType frontType, SqlSugar.DbType behindType, tableInfo info)
        {
            string res = "VARCHAR2(255)";
            switch (frontType, behindType)
            {
                default:
                case (SqlSugar.DbType.MySql, SqlSugar.DbType.Oracle):
                    switch (info.columnType)
                    {
                        case "varchar":
                            res = $"VARCHAR2({Math.Min(info.columnLength, 4000)})";
                            break;
                        case "int":
                        case "bigint":
                            res = $"NUMBER({info.columnNumberPrecision},{info.columnNumberScale})";
                            break;
                        case "text":
                            res = "CLOB";
                            break;
                        case "mediumtext":
                            res = "CLOB";
                            break;
                        case "datetime":
                            res = "DATE";
                            break;
                        default:
                            res = $"VARCHAR2(255)";
                            break;
                    }
                    break;
            }
            return res;
        }
    }*/
    #endregion
    #region 动态语法执行
    /*
    public class Program
    {
        static Action<string> Write = Console.WriteLine;
        public static void Main(string[] args)
        {
            List<Student> list = new();
            list.Add(new Student { Name = "demo1"});
            list.Add(new Student { Name = "demo2" });
            list.Add(new Student { Name = "demo3" });
            list.Add(new Student { Name = "demo4" });
            Console.Write("123");
            DynStatement dynStatement = new DynStatement();
            List<Student> res = dynStatement.executeCode(list)??new List<Student>();
            foreach(Student student in res)
            {
                Write(student.Id.ToString());
                Write(",");
            }
        }
    }*/
    #endregion
    #region miniExcel执行查询
    /*
    internal class Program
    {
        public static void Main(string[] args)
        {
            var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.191.21.53)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=baizedev)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;connect timeout = 600;";
            //var conn = "Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = jxbaizedb-scan1.luxshare.com.cn)(PORT = 1521))) (CONNECT_DATA =(SERVICE_NAME = baizedb))); Persist Security Info=True;User ID=ledrpt;Password=ledrpt;connect timeout = 600;";
            var sql = "SELECT id,WORKORDER ,partid,LINENAME ,CONFIG ,STEPNAME ,PANEL , UNITID ,STATUS ,TRACKTIME ,EQPID ,CREATETIME \r\nFROM LEDRPT.RPT_UNIT_TRACKOUT_DETAIL rutd \r\nWHERE ROWNUM < 1000";
            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = conn,
                DbType = SqlSugar.DbType.Oracle
            });
            var sw = new Stopwatch();
            sw.Start();
            var dt = sqlSugarClient.Ado.GetDataTable(sql);
            //var page = sqlSugarClient.Queryable<Student>().ToPageList(1, 30);
            int total = 0;
            //sqlSugarClient.Queryable<Object>().ToPageList;
            //var dt = sqlSugarClient.SqlQueryable<Object>(sql).ToDataTablePage(1,30);
            Console.WriteLine($"耗时:{sw.ElapsedMilliseconds}");
            var str = DateTime.Now.ToString("O");
            var path = AppDomain.CurrentDomain.BaseDirectory + $"/excel/{Guid.NewGuid()}.xlsx";
            var tempPath = AppDomain.CurrentDomain.BaseDirectory + "/template/aa.xlsx";
            var value = new Dictionary<string, object>()
            {
                ["managers"] = dt
            };
            MiniExcel.SaveAsByTemplate(path, tempPath, value);
            //MiniExcel.SaveAs(path ,dt);

            sw.Stop();
            Console.WriteLine($"耗时:{sw.ElapsedMilliseconds / 1000}");
        }
    }*/
    #endregion
    #region 卡控方法的执行时间
    /*
    internal class Program
    {
        public static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            Console.WriteLine("开始时间:" + startTime);
            var ret = Process(null, 10000);//如果运行时间超过10秒，退出执行
            Console.WriteLine("Result={0}，函数执行时间={1}s", ret, (DateTime.Now - startTime).TotalSeconds);
            startTime = DateTime.Now;
            ret = Process(null, 4000);//如果运行时间超过4秒，退出执行
            Console.WriteLine("Result={0}，函数执行时间={1}s", ret, (DateTime.Now - startTime).TotalSeconds);
            Console.WriteLine("End:" + DateTime.Now);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// 控制执行时间方法
        /// </summary>
        /// <param name="param"></param>
        /// <param name="timeout">程序执行时间</param>
        /// <returns></returns>
        private static bool Process(string param, int timeout)
        {
            var ret = false;
            new System.Threading.Tasks.TaskFactory().StartNew(() =>
            {
                ret = LongTimeFunc();
            }).Wait(timeout);

            return ret;
        }

        /// <summary>
        /// 测试程序，固定执行五秒
        /// </summary>
        /// <returns></returns>
        private static bool LongTimeFunc()
        {
            System.Threading.Thread.Sleep(5000);
            return true;
        }
    }*/
    #endregion
    #region 正则判断
    /*
    internal class Program
    {
        static DataTable dt = new DataTable();

        static void Main(string[] args)
        {
            string function = "BLN2998";
            var (row,column) = translateCoordinate(function);
            Console.WriteLine(row );
            Console.WriteLine("-----------------------");
            Console.WriteLine(column);
            #region 注释
            string aa = "c1";
            bool flag = functionCheck(aa);
            Console.WriteLine(flag);
            //string sql = @"SELECT WORKORDER ,unitid,PARTNAME ,routename,LINENAME ,CURPROCESSNAME ,EQPID ,CURRENTSTATUS , OUTPROCESSTIME,PANELNO ,RULENAME 
            //                FROM GITEADEV.WF_UNITINFOTRAVEL wu   ";
            //ThreadLocal.RunTaskActivity(sql);
            //ThreadLocal.RunTaskSql(sql);
            // var list = sqlThread.list;
            //multiThreadQuery(sql); 
            #endregion
        }

        private static (int, int) translateCoordinate(string function)
        {
            int pow = 0;
            int column = 0;
            string scale = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = function.Length - 1; i >= 0; i--)
            {
                if (!integerCheck(function[i].ToString()))
                {
                    column += (int)Math.Pow(26, pow) * (scale.IndexOf(function[i])+1);
                    pow++;
                }
            }
            int row = Convert.ToInt32(function.Substring(pow));
            return (row-1, column-1);

        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        private static Boolean integerCheck(string value)
        {
            return Regex.IsMatch(value, @"^(-?\d+)(\.\d+)?$");
        }

        /// <summary>
        /// 判断是否只包含字母和数字并且同时包含
        /// </summary>
        private static Boolean functionCheck(string value)
        {
            if(Regex.IsMatch(value, @"^[a-zA-Z0-9]*$"))
                return Regex.IsMatch(value, @"^(?![^\d]+$)(?![^a-zA-Z]+$)[^\u4e00-\u9fa5\s]+$");
            else
                return false;
        }

        private static void multiThreadQuery(string sql)
        {
            var sw = new Stopwatch();
            sw.Start();
            ThreadPool.SetMaxThreads(2, 10);
            int num = getCount(sql);
            for(int i = 0; i < num; i++)
            {

                sqlThread st = new sqlThread
                {
                    sql = sql,
                    startRow = i*50000,
                    endRow = (i+1) * 50000
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(st.executeSql),i);
            }

            while (true)
            {
                if(ThreadPool.ThreadCount <= 0 || sqlThread.list.Count == num)
                {
                    sw.Stop();
                    Console.WriteLine($"执行所有线程所花时间{sw.ElapsedMilliseconds/1000}");
                    Thread.Sleep(1000);
                }else
                {

                }
            }

        }


        private static int getCount(string sql)
        {
            string twoSql = $@"select count(1) counts from ( {sql} )  a";
            DataTable dt = sqlThread.down(twoSql,-1);
            return Convert.ToInt32(dt.Rows[0][0])/ 50000 + 1;
        }

        static bool IsFloat(string str)
        {
            string regextext = @"^[a-zA-Z0-9]*$";
            Regex regex = new Regex(regextext, RegexOptions.None);
            return regex.IsMatch(str.Trim());
        }


    }*/
    #endregion
}
