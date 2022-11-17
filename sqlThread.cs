
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm
{
    using SqlSugar;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    public class sqlThread
    {
        public static ConcurrentBag<DataTable> list { get; set; } = new ConcurrentBag<DataTable>();

        public string sql { get; set; }

        public int startRow { get; set; }

        public int endRow { get; set; }

        public void executeSql(object i)
        {
            Console.WriteLine($"第{i}个线程开始执行{startRow}{endRow}");
            string threeSql = $@"SELECT * FROM (
                                    SELECT rownum rn,a.* FROM ( 
                                        {sql}
                                    )  a 
                                ) a WHERE rn between {startRow} and {endRow}";
            //Console.WriteLine(threeSql);
            list.Add(down(threeSql,(int)i));
            Console.WriteLine($"第{i}个线程执行完毕");
        }

        public static DataTable down(string sql,int i)
        {
            var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.191.21.53)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=baizedev)));Persist Security Info=True;User ID=ledrpt;Password=ledrpt;connect timeout = 600;";

            var sqlSugarClient = new SqlSugarClient(new ConnectionConfig
            {
                ConfigId = "A",
                ConnectionString = conn,
                DbType = SqlSugar.DbType.Oracle
            });
            var sw = new Stopwatch();
            sw.Start();
            /*var sql = @"SELECT WORKORDER ,unitid,PARTID ,CONFIG ,LINENAME ,STEPNAME ,EQPID ,STATUS ,TRACKTIME ,DAYSHIFT ,PANEL ,BOARDNO
FROM LEDRPT.RPT_UNIT_TRACKOUT_DETAIL WHERE workorder = '011001451939'";*/
            Console.WriteLine($"执行sql{i}");
            var dt = sqlSugarClient.Ado.GetDataTable(sql);
            sw.Stop();
            Console.WriteLine($"第{i}个线程耗时:{sw.ElapsedMilliseconds / 1000},Rows={dt.Rows.Count}");

            
            return dt;
        }

    }
}
