

using Microsoft.CodeAnalysis.CSharp.Syntax;
using MiniExcelLibs;
using SqlSugar;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace testorm
{
    /*/// <summary>
    /// 数据库搬迁从mysql 到 oracle
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            string tableName = "auto_turn";
            var mysqlConn = "Server=10.32.44.78;User ID=report01;Password=Baize.2022;port=3306;Database=aj_report;CharSet=utf8;pooling=true;SslMode=None;";
            var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=jxbaizedb-scan1.luxshare.com.cn)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baizedb)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;";
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = mysqlConn,
                DbType = SqlSugar.DbType.MySql
            });
            // 生成建表语句
            List<string> sqls = new();
            var tableSql = $@"SELECT   TABLE_NAME tableName
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
                            where TABLE_NAME = '{tableName}'";
            List<tableInfo> list = sqlSugarClient.Ado.SqlQuery<tableInfo>(tableSql);
            if(list.Count == 0)
            {
                throw new Exception("表不存在！");
            }
            StringBuilder tableCreat = new StringBuilder("CREATE TABLE ");
            tableCreat.Append(list.First().tableName.ToUpper());
            tableCreat.Append(" (");
            foreach(var item in list)
            {
                tableCreat.Append('\n');
                tableCreat.Append("     ");
                tableCreat.Append('"');
                tableCreat.Append(item.columnName.ToUpper());
                tableCreat.Append('"');
                tableCreat.Append(" ");
                tableCreat.Append(getType(SqlSugar.DbType.MySql, SqlSugar.DbType.Oracle,item));
                tableCreat.Append(" ");
                tableCreat.Append(item.columnIsNullable == "YES"? "" : "NOT NULL ");
                tableCreat.Append(" ");
                tableCreat.Append(item.columnDefault==null?"": $" DEFAULT {item.columnDefault} ");
                tableCreat.Append(" ,");
            }
            tableCreat.Append(getConstraint(list));
            tableCreat.Remove(tableCreat.Length - 1, 1);
            tableCreat.Append('\n');
            tableCreat.Append(')');
            sqls.Add(tableCreat.ToString());
            // 注释\

            foreach(var item in list)
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
            var sql = $"select * from {tableName}";
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
            using (sqlSugarClient2.Ado.OpenAlways())
            { //开启长连接

                foreach (var str in sqls)
                {
                    sqlSugarClient2.Ado.ExecuteCommand(str);
                }

            }
            //int a = sqlSugarClient2.Ado.ExecuteCommand(tableCreat.ToString());

            sqlSugarClient2.Fastest<System.Data.DataTable>().AS( tableName.ToUpper()).BulkCopy(dt);
            sqlSugarClient2.Close();
            sqlSugarClient2.Dispose();
        }

        private static string getConstraint(List<tableInfo> list)
        {
            StringBuilder res = new StringBuilder();
            IEnumerable<string> keys =  list.Where(x=>!string.IsNullOrEmpty(x.columnConstraint)).Select(x => x.columnConstraint).Distinct();
            foreach(var key in keys)
            {
                switch (key)
                {
                    case "PRI":
                        res .Append($"\nPRIMARY KEY (\"{list.Where(x => x.columnConstraint == key).First().columnName.ToUpper()}\") ,");
                        break;
                    case "UNI":
                        IEnumerable<string> names =  list.Where(x => x.columnConstraint == key).Select(x => x.columnName);
                        if(names.Count() == 1)
                        {
                            res.Append($"\nCONSTRAINT {list.Where(x => x.columnConstraint == key).First().tableName.ToUpper()}_U1 UNIQUE (\"{names.First().ToUpper()}\"),");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach(string name in names)
                            {
                                sb.Append('"');
                                sb.Append(name);
                                sb.Append('"');
                                sb.Append(',');
                            }
                            res.Append($"CONSTRAINT constraint_name UNIQUE ({sb.Remove(sb.Length-1,1)})");
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
            switch(frontType, behindType)
            {
                default:
                case (SqlSugar.DbType.MySql, SqlSugar.DbType.Oracle):
                    switch (info.columnType)
                    {
                        case "varchar":
                            res = $"VARCHAR2({Math.Min(info.columnLength,4000)})";
                            break;
                        case "int":
                            res = $"NUMBER({info.columnNumberPrecision},{info.columnNumberScale})";
                            break;
                        case "text":
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

    /*/// <summary>
    /// 动态语法执行
    /// </summary>
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

    /*/// <summary>
    /// miniExcel执行查询
    /// </summary>
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

    /*/// <summary>
    /// 卡控方法的执行时间
    /// </summary>
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

    /*/// <summary>
    /// 正则判断
    /// </summary>
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
}
