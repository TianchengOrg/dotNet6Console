using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm
{
    internal class ThreadLocal
    {
        /// <summary>
        /// 分页查询数据库 以100万为基准
        /// </summary>
        public static void RunTaskActivity(string sql, int maxThread = 10,int totalThread = 40)
        {
            // 获取要取得数据总数
            int total = getCount(sql);
            int singleTaskNumber = Math.Max(total/ totalThread-1,50000);
            List<Task> allTask = new();
            int totalTaskCount = total / singleTaskNumber + (total % singleTaskNumber == 0 ? 0 : 1);
            int circNumber = totalTaskCount / maxThread + (totalTaskCount % maxThread == 0 ? 0 : 1);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < totalThread; i++)
            {
                sqlThread st = new sqlThread
                {
                    sql = sql,
                    startRow =  + i * singleTaskNumber,
                    endRow = Math.Min(  (i + 1) * singleTaskNumber, total) - 1
                };
                if (st.startRow > st.endRow)
                    break;
                int index = i;

                if(allTask.Count >= maxThread)
                {
                    while (allTask.Where(x=> x.Status == TaskStatus.Running || x.Status == TaskStatus.WaitingToRun).Count()>= maxThread)
                    {
                        // 关闭不需要的线程
                        Thread.Sleep(100);
                        allTask = allTask.Where(x => x.Status == TaskStatus.Created || x.Status == TaskStatus.Running || x.Status == TaskStatus.WaitingToRun).ToList();
                    }
                    
                }
                    
                allTask.Add(Task.Run(() =>
                {
                    st.executeSql(index);

                }));
            };
            // Task.WaitAll(allTask.ToArray());
            
            Task.WaitAll(allTask.ToArray());
            sw.Stop();
            Console.WriteLine($"耗时{sw.ElapsedMilliseconds / 1000}s");
        }

        /// <summary>
        /// 分页查询数据库 以100万为基准
        /// </summary>
        public static void RunTaskSql( string sql, int maxThread = 10,int singleTaskNumber = 10000 )
        {
            // 获取要取得数据总数
            int total = getCount(sql);
            List<Task> allTask = new();
            int totalTaskCount = total / singleTaskNumber + (total % singleTaskNumber == 0 ? 0 : 1);
            int circNumber = totalTaskCount / maxThread + (totalTaskCount % maxThread == 0 ? 0 : 1);

            var sw = new Stopwatch();
            sw.Start();
            for (int j = 0;j<circNumber; j++)
            {
                for (int i = 0; i < maxThread; i++)
                {
                    sqlThread st = new sqlThread
                    {
                        sql = sql,
                        startRow = j * maxThread * singleTaskNumber + i * singleTaskNumber,
                        endRow = Math.Min(j * maxThread * singleTaskNumber + (i + 1) * singleTaskNumber, total)-1
                    };
                    if (st.startRow > st.endRow)
                        break;
                    int index = j * maxThread + i;
                    allTask.Add(Task.Run(() =>
                    {
                        st.executeSql(index);

                    }));
                };
                Task.WaitAll(allTask.ToArray());
            }
            sw.Stop();
            Console.WriteLine($"耗时{sw.ElapsedMilliseconds / 1000}s");
        }
        public static void RunTask<T>(IList<T> preList, Action<List<T>> DealWithData, int maxThread = 10)
        {
            int alreadyHander = 0;                                     //已处理线程数
            int handlerCount = maxThread;                              //线程上限数                                   
            int alreadyCount = 0;                                      //已处理数据量
            int singleTaskNumber = preList.Count / handlerCount + (preList.Count % handlerCount == 0 ? 0 : 1);
            List<Task> allTask = new();

            for (int i = 0; i < handlerCount; i++)
            {
                var list = preList.Skip(alreadyHander * singleTaskNumber).Take(singleTaskNumber).ToList();
                if (list.Any())
                {
                    allTask.Add(Task.Run(() =>
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        DealWithData(list);
                        sw.Stop();
                        Console.WriteLine($"线程执行完成，数量{list.Count}， 当前执行{alreadyCount += list.Count}，剩余{preList.Count - alreadyCount}，耗时{sw.ElapsedMilliseconds/1000}ms");

                    }));
                }
                alreadyHander++;
            };
            Task.WaitAll(allTask.ToArray());
        }

        /// <summary>
        /// 每个线程执行固定的数量
        /// </summary>
        /// <typeparam name="T">处理类</typeparam>
        /// <param name="preList">数据</param>
        /// <param name="DealWithData">处理数据的方法</param>
        public static void RunTaskByList<T>(IList<T> preList, Action<List<T>> DealWithData, int maxThread = 100, int pageSize = 500)
        {
            var page = preList.Count / pageSize + (preList.Count % pageSize == 0 ? 0 : 1);
            var alreadyHander = 0;
            var handlerCount = maxThread;
            var allTask = new List<Task>();
            var allPageIndexArray = new List<int>();
            for (int i = 0; i < page; i++)
            {
                allPageIndexArray.Add(i);
            }
            while (alreadyHander < page)
            {
                allPageIndexArray.Skip(alreadyHander).Take(handlerCount).ToList().ForEach(x =>
                {
                    var list = preList.Skip(alreadyHander * pageSize).Take(pageSize).ToList();
                    allTask.Add(Task.Run(() =>
                    {
                        DealWithData(list);
                    }));
                    alreadyHander++;
                });
                Task.WaitAll(allTask.ToArray());

            }
        }

        /// <summary>
        /// 根据sql获取count
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static int getCount(string sql)
        {
            string twoSql = $@"select count(1) counts from ( {sql} )  a";
            DataTable dt = sqlThread.down(twoSql, -1);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
    }
}
