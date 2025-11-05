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
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Net;
using System.Security.Policy;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Buffers.Text;
using testorm.utils;
using System.Net.Http.Headers;
using Razorvine.Pyro;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Spark.Sql;
using static System.Formats.Asn1.AsnWriter;
using NodaTime;
using testorm.Calculator;
using RabbitMQ.Client;

namespace testorm;

using Apache.Arrow;
using SqlSugar;
using System.Globalization;

/*public class program
{
    public static void Main(string[] args)
    {
        
    }
}*/

#region 根据日期获取阴历日期
public class program
{
    static void Main()
    {
        var date = new DateTime(2025, 11, 5, 0, 0, 0, DateTimeKind.Local);
        var cal = new ChineseLunisolarCalendar();

        int year = cal.GetYear(date);
        int month = cal.GetMonth(date);
        int day = cal.GetDayOfMonth(date);

        int leapMonth = cal.GetLeapMonth(year);

        var isLeap = leapMonth == month;

        if (leapMonth > 0 && month >= leapMonth)
            month--;

        Console.WriteLine($"公历: {date:yyyy-MM-dd}");
        Console.WriteLine($"农历年: {year}");
        Console.WriteLine($"农历月: {month} {(isLeap ? "(闰)" : "")}");
        Console.WriteLine($"农历日: {day}");
    }
}
#endregion
#region mes 接口同步wms 物料条码的信息
/*public class program
{
    public static async Task Main(string[] args)
    {
        List<ProductInfo> infos = new List<ProductInfo>();
        string connect = "Server=10.11.26.220;Database=wms;User Id=sa;Password=Knt@654321;Encrypt=True;TrustServerCertificate=True";
        SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
        {
            ConfigId = "A",
            ConnectionString = connect,
            DbType = SqlSugar.DbType.SqlServer,
            IsAutoCloseConnection = true
        });
        var dt = sqlSugarClient.Ado.GetDataTable(@"select * from  wms.dbo.InBoundDetail 
where CreateTime  > CAST('2025-04-24 17:00:00' as datetime)
order by CreateTime desc");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Thread.Sleep(1000);
            //Console.WriteLine(dt.Rows[i]["LastProcessCode"].ToString());
            //Console.WriteLine(dt.Rows[i]["LastInStationTime"].ToString());
            //if (dt.Rows[i]["LastProcessCode"].ToString() == null || dt.Rows[i]["LastProcessCode"].ToString() == ""||
            //    dt.Rows[i]["LastInStationTime"].ToString() == null || dt.Rows[i]["LastInStationTime"].ToString() == "")
            if (dt.Rows[i]["BarCode"].ToString().Length == 34)
            {
                Console.WriteLine("-- " + dt.Rows[i]["BarCode"]);
            }
            else
            {
                continue;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {


                    // 设置请求的 URL
                    string url = "http://10.11.38.100:10023/MainStatus/GetList?BarCode=" + dt.Rows[i]["BarCode"]; // 替换为实际 API 地址

                    // 发送 GET 请求并获取响应
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 确保请求成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容为字符串
                    string jsonResponse = await response.Content.ReadAsStringAsync();


                    // 将 JSON 字符串反序列化为实体对象
                    infos = JsonSerializer.Deserialize<List<ProductInfo>>(jsonResponse);

                    //JsonFileHelper.SaveToJson<List<ProductInfo>>(infos,"productInfo.json");

                    // 输出结果
                    //Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, Age: {person.Age}");

                    var tmp = JsonFileHelper.LoadFromJson<List<ProductInfo>>("productInfo.json");
                }
                catch (HttpRequestException e)
                {
                    // 处理 HTTP 请求异常
                    Console.WriteLine($"HTTP 请求错误: {e.Message}");
                }
                catch (JsonException e)
                {
                    // 处理 JSON 反序列化异常
                    Console.WriteLine($"JSON 反序列化错误: {e.Message}");
                }
            }

            if (infos.Any())
            {
                var str = JsonSerializer.Serialize(infos);
                // Console.WriteLine(str);
                long id = (long)dt.Rows[i]["Id"];
                Console.WriteLine("-- " + dt.Rows[i]["Id"].ToString());
                string endtime = "null";//668567102976069
                if (infos[0].endTime != null)
                    endtime = $"CAST('{infos[0].endTime.ToString()}' as datetime)";
                if (dt.Rows[i]["Id"].ToString().Length != 15 || dt.Rows[i]["BarCode"].ToString().Length != 34)
                {
                    continue;
                }
                string sql = $"update wms.dbo.InBoundDetail set LastProcessCode='{infos[0].startProcessCode}',LastProcessName='{infos[0].startProcessName}',LastInStationTime={endtime},QualityInfo={infos[0].qualityStatus}  where id = {id} and barcode='{dt.Rows[i]["BarCode"].ToString()}';";
                Console.WriteLine(sql);
            }
        }
    }
}*/
#endregion
#region  发送http 请求
/*public class program
{
    static async Task Main(string[] args)
    {
        // 创建 HttpClient 实例
        using (HttpClient client = new HttpClient())
        {
            try
            {


                // 设置请求的 URL
                string url = "http://10.11.38.100:10023/MainStatus/GetList?BarCode=T1BA100024000157000000003240810A"; // 替换为实际 API 地址

                // 发送 GET 请求并获取响应
                HttpResponseMessage response = await client.GetAsync(url);

                // 确保请求成功
                response.EnsureSuccessStatusCode();

                // 读取响应内容为字符串
                string jsonResponse = await response.Content.ReadAsStringAsync();


                // 将 JSON 字符串反序列化为实体对象
                List<ProductInfo> infos = JsonSerializer.Deserialize<List<ProductInfo>>(jsonResponse);

                //JsonFileHelper.SaveToJson<List<ProductInfo>>(infos,"productInfo.json");

                // 输出结果
                //Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, Age: {person.Age}");

                var tmp = JsonFileHelper.LoadFromJson<List<ProductInfo>>("productInfo.json");
            }
            catch (HttpRequestException e)
            {
                // 处理 HTTP 请求异常
                Console.WriteLine($"HTTP 请求错误: {e.Message}");
            }
            catch (JsonException e)
            {
                // 处理 JSON 反序列化异常
                Console.WriteLine($"JSON 反序列化错误: {e.Message}");
            }
        }
    }
}*/
#endregion
#region 调用rabitmq 发送sn信息 记录日志
//internal class Program
//{
//    static void Main(string[] args)
//    {
//        // RabbitMQ 服务器配置
//        var factory = new ConnectionFactory
//        {
//            HostName = "10.11.22.235", // RabbitMQ 服务器地址
//            UserName = "wms",     // 默认用户名
//            Password = "qwaszx123"      // 默认密码
//        };

//        // 创建连接和通道
//        using (var connection = factory.CreateConnectionAsync().Result)
//        using (var channel = connection.CreateChannelAsync().Result)
//        {
//            // 声明队列
//            string queueName = "wms_log";
//            var res = channel.QueueDeclareAsync(queue: queueName,
//                                 durable: false,   // 队列是否持久化
//                                 exclusive: false, // 是否为排他队列
//                                 autoDelete: false,// 是否自动删除
//                                 arguments: null).Result;

//            Console.WriteLine("请输入要发送的消息（输入 'exit' 退出）：");

//            while (true)
//            {
//                // 获取用户输入
//                string message = Console.ReadLine();
//                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
//                    break;

//                // 将消息转换为字节数组
//                var body = Encoding.UTF8.GetBytes(message);

//                // 发布消息到队列
//                channel.BasicPublishAsync(exchange: "",       // 默认交换机
//                                     routingKey: queueName, // 队列名称
//                                     mandatory: true,
//                                     body: body);

//                Console.WriteLine($"消息已发送：{message}");
//            }
//        }

//        Console.WriteLine("生产者已退出。");
//    }
//}
#endregion
#region 调用rabitmq 发送信息立库出库
//internal class Program
//{
//    static void Main(string[] args)
//    {
//        // RabbitMQ 服务器配置
//        var factory = new ConnectionFactory
//        {
//            HostName = "10.11.22.235", // RabbitMQ 服务器地址
//            UserName = "wms",     // 默认用户名
//            Password = "qwaszx123"      // 默认密码
//        };

//        // 创建连接和通道
//        using (var connection = factory.CreateConnectionAsync().Result)
//        using (var channel = connection.CreateChannelAsync().Result)
//        {
//            // 声明队列
//            string queueName = "wms_stand_out";
//            var res = channel.QueueDeclareAsync(queue: queueName,
//                                 durable: false,   // 队列是否持久化
//                                 exclusive: false, // 是否为排他队列
//                                 autoDelete: false,// 是否自动删除
//                                 arguments: null).Result;

//            Console.WriteLine("请输入要发送的消息（输入 'exit' 退出）：");

//            while (true)
//            {
//                // 获取用户输入
//                string message = Console.ReadLine();
//                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
//                    break;

//                // 将消息转换为字节数组
//                var body = Encoding.UTF8.GetBytes(message);

//                // 发布消息到队列
//                channel.BasicPublishAsync(exchange: "",       // 默认交换机
//                                     routingKey: queueName, // 队列名称
//                                     mandatory: true,
//                                     body: body);

//                Console.WriteLine($"消息已发送：{message}");
//            }
//        }

//        Console.WriteLine("生产者已退出。");
//    }
//}
#endregion
#region 雪花ID
/*internal class Program
{
    static void Main(string[] args)
    {
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine( "开始执行 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + "    " + Snowflake.Instance().GetId());
        }
    }
}

/// <summary>
/// 动态生产有规律的ID
/// </summary>
public class Snowflake
{
    private static long machineId;//机器ID
    private static long datacenterId = 0L;//数据ID
    private static long sequence = 0L;//计数从零开始

    private static long twepoch = 687888001020L; //唯一时间随机量

    private static long machineIdBits = 5L; //机器码字节数
    private static long datacenterIdBits = 5L;//数据字节数
    public static long maxMachineId = -1L ^ -1L << (int)machineIdBits; //最大机器ID
    private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大数据ID

    private static long sequenceBits = 12L; //计数器字节数，12个字节用来保存计数码        
    private static long machineIdShift = sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
    private static long datacenterIdShift = sequenceBits + machineIdBits;
    private static long timestampLeftShift = sequenceBits + machineIdBits + datacenterIdBits; //时间戳左移动位数就是机器码+计数器总字节数+数据字节数
    public static long sequenceMask = -1L ^ -1L << (int)sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
    private static long lastTimestamp = -1L;//最后时间戳

    private static object syncRoot = new object();//加锁对象
    static Snowflake snowflake;

    public static Snowflake Instance()
    {
        if (snowflake == null)
            snowflake = new Snowflake();
        return snowflake;
    }

    public Snowflake()
    {
        Snowflakes(0L, -1);
    }

    public Snowflake(long machineId)
    {
        Snowflakes(machineId, -1);
    }

    public Snowflake(long machineId, long datacenterId)
    {
        Snowflakes(machineId, datacenterId);
    }

    private void Snowflakes(long machineId, long datacenterId)
    {
        if (machineId >= 0)
        {
            if (machineId > maxMachineId)
            {
                throw new Exception("机器码ID非法");
            }
            Snowflake.machineId = machineId;
        }
        if (datacenterId >= 0)
        {
            if (datacenterId > maxDatacenterId)
            {
                throw new Exception("数据中心ID非法");
            }
            Snowflake.datacenterId = datacenterId;
        }
    }

    /// <summary>
    /// 生成当前时间戳
    /// </summary>
    /// <returns>毫秒</returns>
    private static long GetTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    /// <summary>
    /// 获取下一微秒时间戳
    /// </summary>
    /// <param name="lastTimestamp"></param>
    /// <returns></returns>
    private static long GetNextTimestamp(long lastTimestamp)
    {
        long timestamp = GetTimestamp();
        if (timestamp <= lastTimestamp)
        {
            timestamp = GetTimestamp();
        }
        return timestamp;
    }

    /// <summary>
    /// 获取长整形的ID
    /// </summary>
    /// <returns></returns>
    public long GetId()
    {
        lock (syncRoot)
        {
            long timestamp = GetTimestamp();
            if (Snowflake.lastTimestamp == timestamp)
            { //同一微妙中生成ID
                sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                if (sequence == 0)
                {
                    //一微妙内产生的ID计数已达上限，等待下一微妙
                    timestamp = GetNextTimestamp(Snowflake.lastTimestamp);
                }
            }
            else
            {
                //不同微秒生成ID
                sequence = 0L;
            }
            if (timestamp < lastTimestamp)
            {
                throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
            }
            Snowflake.lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
            long Id = ((timestamp - twepoch) << (int)timestampLeftShift)
                | (datacenterId << (int)datacenterIdShift)
                | (machineId << (int)machineIdShift)
                | sequence;
            return Id;
        }
    }
}*/
#endregion
#region 网络抓包
//    internal class Program
//{
//        static void Main(string[] args)
//        {
//            // 目标 IP 地址
//            string targetIp = "10.11.38.100";
//            // 要检查的指定字符
//            string targetString = "T1BA100094000";

//            // 获取所有网络设备
//            var devices = CaptureDeviceList.Instance;

//            if (devices.Count == 0)
//            {
//                Console.WriteLine("未找到网络设备！");
//                return;
//            }

//            // 选择第一个网络设备（可以根据需要选择特定设备）
//            var device = devices[3];

//            // 打开设备
//            device.Open(DeviceModes.Promiscuous, 1000);

//            Console.WriteLine($"开始捕获 {device.Description} 的网络数据包...");

//            // 注册数据包捕获事件
//            device.OnPacketArrival += (sender, e) =>
//            {
//                var packet = Packet.ParsePacket(  e.Device.LinkType, e.Data.ToArray() );

//                // 检查是否为 IP 数据包
//                var ipPacket = packet.Extract<IPPacket>();
//                if (ipPacket != null)
//                {
//                    // 检查源 IP 或目标 IP 是否为 10.11.38.100
//                    if (ipPacket.SourceAddress.ToString() == targetIp || ipPacket.DestinationAddress.ToString() == targetIp)
//                    {
//                        Console.WriteLine("捕获到指定IP地址信息的内同交互");
//                        // 提取 TCP 或 UDP 数据包
//                        var tcpPacket = packet.Extract<TcpPacket>();
//                        var udpPacket = packet.Extract<UdpPacket>();

//                        if (tcpPacket != null || udpPacket != null)
//                        {
//                            // 获取负载数据
//                            byte[] payload = tcpPacket?.PayloadData ?? udpPacket?.PayloadData;

//                            if (payload != null)
//                            {
//                                // 将负载数据转换为字符串
//                                string payloadString = Encoding.UTF8.GetString(payload);

//                                // 检查是否包含指定字符
//                                if (payloadString.Contains(targetString))
//                                {
//                                    Console.WriteLine($"发现目标字符串: {targetString}");
//                                    Console.WriteLine($"数据包内容: {payloadString}");
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            // 开始捕获数据包
//            device.StartCapture();

//            Console.WriteLine("按 Enter 键停止捕获...");
//            Console.ReadLine();

//            // 停止捕获并关闭设备
//            device.StopCapture();
//            device.Close();
//        }

//}
#endregion
#region WebSocket 
/// <summary>
///  websocket
/// </summary>
//internal class Program
//{
//    static void Main(string[] args)
//    {
//        InitializeWebSocketServer();

//        Console.WriteLine("按任意键停止服务器...");
//        Console.ReadKey();
//    }

//    private static void InitializeWebSocketServer()
//    {
//        // 创建 WebSocket 服务器实例，并指定端口号
//        var wssv = new WebSocketServer("ws://10.11.22.11:5000");

//        // 添加 WebSocket 服务行为
//        wssv.AddWebSocketService<ChatBehavior>("/chat");

//        // 启动 WebSocket 服务器                  
//        wssv.Start();

//        if (wssv.IsListening)
//        {
//            Console.WriteLine($"WebSocket 服务器已启动，监听地址: {wssv.Address}:{wssv.Port}");
//        }
//        else
//        {
//            Console.WriteLine("WebSocket 服务器启动失败");
//        }

//        // 注册服务器关闭事件
//        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
//        {
//            wssv.Stop();
//            Console.WriteLine("WebSocket 服务器已停止");
//        };
//    }
//}

//// 自定义 WebSocket 行为类
//public class ChatBehavior : WebSocketBehavior
//{
//    protected override void OnOpen()
//    {
//        Console.WriteLine("客户端已连接");
//        Sessions.Broadcast("欢迎来到聊天室！");
//    }

//    protected override void OnMessage(MessageEventArgs e)
//    {
//        Console.WriteLine($"收到消息: {e.Data}");

//        // 将接收到的消息广播给所有连接的客户端
//        Sessions.Broadcast($"客户端说: {e.Data}");
//    }

//    protected override void OnClose(CloseEventArgs e)
//    {
//        Console.WriteLine("客户端已断开连接");
//        Sessions.Broadcast("一个客户端离开了聊天室。");
//    }

//    protected override void OnError(WebSocketSharp.ErrorEventArgs e)
//    {
//        Console.WriteLine($"发生错误: {e.Message}");
//    }

//}

#endregion
#region 连接PLC
//internal class Program
//{
//    // 定义PLC对象
//    private static Plc plc;
//    static void Main(string[] args)
//    {
//        plc = new Plc(CpuType.S71200, "192.168.0.1", 0, 1);
//        try
//        {
//            plc.Open();
//            if (plc.IsConnected)
//                Console.WriteLine("PLC连接成功");
//            else
//                Console.WriteLine("PLC连接失败");

//            var res = plc.ReadAsync("DB68.DBX0.1").Result;
//            Console.WriteLine("上料trigger读取成功："+res.ToString());

//            //plc.Write("DB68.DBX0.1", true);
//            //plc.Write(DataType.DataBlock, 68, 0, true);

//            //plc.Write(DataType.DataBlock, 68, 260, true);
//            //plc.Write(DataType.DataBlock, 68, 264, false);
//            plc.Write(DataType.DataBlock, 68, 1556, false);
//            Console.WriteLine("写入成功");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("PLC连接失败" + ex.Message);
//        }finally
//        {
//            Console.WriteLine("关闭PLC连接");
//            plc.Close();
//        }
//    }
//}
#endregion
//#region mes 数据同步
//public class Program
//{
//    static string tableName = "Log_AgvInstructionLog";
//    static bool deleteAll = true;  // false 是伪删除 修改isdeleted 为 1
//    static string[] ignoreArr = ["Id", "CreaterId", "ModifierId","CreaterID", "ModifierID"]; // 忽略字段
//    // 对应字段
//    static List<Cor> corlist = new List<Cor>
//    {
//        new Cor{ key = "CreateTime" ,value = "CreateDate" },
//        new Cor{ key = "ModifyTime" ,value = "ModifyDate" }
//    };

//    static void Main(string[] args)
//    {
//        // 获取旧mes表未删除全部数据
//        SqlSugarClient oldDb = new SqlSugarClient(new ConnectionConfig()
//        {
//            ConnectionString = "Data Source=10.11.38.100;Initial Catalog=KangNaiTuo_MES;Persist Security Info=True;User ID=gepengfei;Password=gepengfei;Connect Timeout=500;Encrypt=false;",
//            DbType = DbType.SqlServer,
//            IsAutoCloseConnection = true
//        });
//        var dt = oldDb.Ado.GetDataTable($"select * from {tableName} where IsDeleted = 0 order by CreateTime asc");
//        // 获取所有需要的列名，排除忽略字段并应用映射
//        var columns = dt.Columns.Cast<DataColumn>()
//                                .Where(c => !ignoreArr.Contains(c.ColumnName))
//                                .Select(c => c.ColumnName)
//                                .ToList();
//        var replaceColumns = columns.Select(c =>
//                                    corlist.FirstOrDefault(cor => cor.key == c)?.value ?? c
//                                );
//        // 根据忽略字段，对应字段创建插入sql中的插入值对应列表
//        // 构建列名字符串和参数占位符字符串

//        var columnNames = string.Join(", ", replaceColumns);

//        // 导入之前先删除之前的数据
//        SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
//        {
//            ConnectionString = "Data Source=10.11.26.220;Initial Catalog=MES;Persist Security Info=True;User ID=sa;Password=Knt@654321;Connect Timeout=500;Encrypt=false;",
//            DbType = DbType.SqlServer,
//            IsAutoCloseConnection = true
//        });
//        if (deleteAll)
//        {
//            string deleteSql = $"delete from {tableName}";
//            var res = Db.Ado.ExecuteCommand(deleteSql);
//            Console.WriteLine($"共删除了{res}条数据");
//        }
//        else
//        {
//            string deleteSql = $"update {tableName} set IsDeleted = 1  where IsDeleted = 0 ";
//            var res = Db.Ado.ExecuteCommand(deleteSql);
//            Console.WriteLine($"共伪删除了{res}条数据");
//        }

//        // 遍历 DataTable 的每一行以生成每行的 SQL 插入语句
//        foreach (DataRow row in dt.Rows)
//        {
//            // 构建参数值
//            var parameters = columns.Select(col =>
//                row[col] == DBNull.Value ? "NULL" : FormatValue(dt, row[col], col))
//                .ToList();

//            // 构建完整的 INSERT INTO 语句
//            var insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({string.Join(", ", parameters)});";

//            // 打印生成的 SQL 语句到控制台
//            Console.WriteLine(insertSql);
//            var res = Db.Ado.ExecuteCommand(insertSql);
//            if (res > 0)
//            {
//                Console.WriteLine($"新增成功{res}");
//            }
//        }
//    }
//    private static string FormatValue(DataTable dt, object value, string columnName)
//    {
//        // 根据列名确定类型并格式化值
//        Type dataType = dt.Columns[columnName].DataType;
//        if (dataType == typeof(string) || dataType == typeof(DateTime))
//        {
//            return $"'{value}'";
//        }
//        else if (dataType == typeof(bool))
//        {
//            return ((bool)value) ? "1" : "0";
//        }
//        else
//        {
//            return value.ToString();
//        }
//    }
//}
//public class Cor
//{
//    public string? key { get; set; }

//    public string? value { get; set; }
//}
//#endregion
#region 手动同步后底板前梁数据
/*internal class Program
{
    static void Main(string[] args)
    {
        SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "Data Source=10.11.26.220;Initial Catalog=MES;Persist Security Info=True;User ID=sa;Password=Knt@654321;Connect Timeout=500;Encrypt=false;",
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true
        });

        SqlSugarClient oldDb = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "Data Source=10.11.38.100;Initial Catalog=KangNaiTuo_MES;Persist Security Info=True;User ID=lijinghui;Password=asd123asd;Connect Timeout=500;Encrypt=false;",
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true
        });


        // 获取所有前梁信息
        var dt = Db.Ado.GetDataTable(@"select * from Basic_BarcodeGenerateDetail 
where CreateDate > CAST(FLOOR(CAST(GETDATE()-2 AS FLOAT)) AS DATETIME)   
and BatchCode = '30100000033' 
order by CreateDate desc ");
        Console.WriteLine(dt);

        //21600000006  前梁
        //30100000033  后底板

        bool flag = false;
        //string name = "前梁";
        //var workorder = "20241022144148";

        string name = "后底板";
        var workorder = "20240904104704";

        // 循环绑定工单信息
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var code = dt.Rows[i]["Code"].ToString();
            var lotcode = dt.Rows[i]["LotCode"].ToString();
            var batchCode = dt.Rows[i]["BatchCode"].ToString();
            if (code.Contains("*"))
                flag = true;
            else
                flag = false;

            if (!flag)
            {
                // 根据总成码判断是否绑定
                string checkSql = $@"select * from StationData_BarCodeBindHistory 
where FinishedProductBarCode = '{code}'";
                var checkDt = oldDb.Ado.GetDataTable(checkSql);
                if(checkDt.Rows.Count != 1)
                {
                    Console.WriteLine("绑定数据异常：数量为 "+ checkDt.Rows.Count+"  编码为:  "+ code);
                }
            }

            long sequenceCode = 0;

            // 获取工单信息  21600000006  
            string liaohao = "";
            string routeCode = "";
            string zrouteCode = "";
            string inLineSql = "";
            string workorderSelect = "";
            int res = 0;

            if (name.Contains("后底板"))
            {
                // 获取工单信息
                liaohao = "20400000053";
                routeCode = "20400000053";
                zrouteCode = "T1BA70007";
                workorderSelect = @$"select 
a.code MaterialCode,a.name MaterialName ,a.WONo,rou.ProductionLineCode LineCode,rou.ProductionLineName LineName,rou.RouteCode,
p.ProcessCode,p.ProcessIdentify,p.ProcessName,p.BackProcessCode,p.BackProcessIdentify,p.BackProcessName,
rou.MaterialTypeCode,rou.MaterialTypeName 
from (
	select wo.MaterialCode,MaterialName, wo.WONo,bm.MaterialTypeCode,bm.MaterialTypeName,bm.code,bm.name  from 
	WorkSchedule_WO  wo , Basic_MaterialCategory bm
	where wo.WONo = '{workorder}' and bm.Code  = '{liaohao}' 
	and wo.WOStatus  = '生产中' and wo.IsDeleted  = 0 
) a  left join WorkOrder_Route rou on rou.WONo  = a.WONo and a.MaterialTypeCode = rou.MaterialTypeCode and rou.RouteCode = '{routeCode}'
left join WorkOrder_RouteProcess p on p.WONo  = a.WONo and p.RouteCode  = rou.RouteCode 
where   p.IsFirstProcess = 1 and p.ProcessSequence = 1";

                //var workorderInfo = DBServerProvider.SqlDapperOldMes.QueryDynamicList(workorderSelect, null);
                var workorderInfo = oldDb.Ado.GetDataTable(workorderSelect);

                if (workorderInfo.Rows.Count != 1)
                {
                    Console.Write($"工单信息[{workorder}]有误，共查询到【 {workorderInfo.Rows.Count} 】条数据");
                    return;
                }

                // 获取mes唯一数字码
                sequenceCode = getSequenceId(Db);
                inLineSql = $@"INSERT INTO DataCollection_MainStatus (Id,BarCode,NumberTypeCode,NumberTypeName,
ContainerLayer,ContainerRow,ContainerColumn,
QualityStatus,RouteCode,LineCode,LineName,WONo,
LastProcessCode,LastProcessIdentify,LastProcessName,NextProcessCode,NextProcessIdentify,NextProcessName,
MaterialTypeCode,MaterialTypeName,MaterialCode,MaterialName,MaterialCount,IsFinishProcess,IsConsumed,IsLock,BatchNo,IsDeleted,CreaterId ,CreateTime) 
VALUES({sequenceCode},N'{code}',N'BatchCode',N'批次号',
0,0,0,N'OK',N'{workorderInfo.Rows[0]["RouteCode"]}',N'{workorderInfo.Rows[0]["LineCode"]}',N'{workorderInfo.Rows[0]["LineName"]}',N'{workorder}',
N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',N'{workorderInfo.Rows[0]["BackProcessCode"]}',N'{workorderInfo.Rows[0]["BackProcessIdentify"]}',N'{workorderInfo.Rows[0]["BackProcessName"]}',
N'{workorderInfo.Rows[0]["MaterialTypeCode"]}',N'{workorderInfo.Rows[0]["MaterialTypeName"]}',N'{workorderInfo.Rows[0]["MaterialCode"]}',N'{workorderInfo.Rows[0]["MaterialName"]}',1,0,0,0,N'{lotcode}',0,4373791262185230336,GETDATE());";

                if (flag)
                {
                    var tmp = checkOldmes(oldDb, code);
                    if (tmp)
                    {
                        Console.WriteLine("同步成功："+ code);
                        res = oldDb.Ado.ExecuteCommand(inLineSql);
                    }
                    continue;
                }


                // 获取mes唯一数字码 总成码导入
                sequenceCode = getSequenceId(Db);
                workorderSelect = @$"select 
a.MaterialCode,a.MaterialName ,a.WONo,rou.ProductionLineCode LineCode,rou.ProductionLineName LineName,rou.RouteCode,
p.ProcessCode,p.ProcessIdentify,p.ProcessName,p.BackProcessCode,p.BackProcessIdentify,p.BackProcessName,
rou.MaterialTypeCode,rou.MaterialTypeName 
from (
	select wo.MaterialCode,MaterialName, wo.WONo,bm.MaterialTypeCode,bm.MaterialTypeName,bm.code,bm.name  from 
	WorkSchedule_WO  wo , Basic_MaterialCategory bm
	where wo.WONo = '{workorder}' and bm.Code  = '{liaohao}' 
	and wo.WOStatus  = '生产中' and wo.IsDeleted  = 0 
) a  left join WorkOrder_Route rou on rou.WONo  = a.WONo and rou.RouteCode = '{zrouteCode}'
left join WorkOrder_RouteProcess p on p.WONo  = a.WONo and p.RouteCode  = rou.RouteCode 
where   p.IsFirstProcess = 1 and p.ProcessSequence = 1";
                workorderInfo = oldDb.Ado.GetDataTable(workorderSelect);

                if (workorderInfo.Rows.Count != 1)
                {
                    Console.Write($"工单信息[{workorder}]有误，共查询到【 {workorderInfo.Rows.Count} 】条数据");
                    return;
                }
                inLineSql = $@"INSERT INTO DataCollection_MainStatus (Id,BarCode,NumberTypeCode,NumberTypeName,
ContainerLayer,ContainerRow,ContainerColumn,
QualityStatus,RouteCode,LineCode,LineName,WONo,
LastProcessCode,LastProcessIdentify,LastProcessName,NextProcessCode,NextProcessIdentify,NextProcessName,
MaterialTypeCode,MaterialTypeName,MaterialCode,MaterialName,MaterialCount,IsFinishProcess,IsConsumed,IsLock,BatchNo,IsDeleted,CreaterId ,CreateTime) 
VALUES({sequenceCode},N'{code}',N'BatchCode',N'批次号',
0,0,0,N'OK',N'{workorderInfo.Rows[0]["RouteCode"]}',N'{workorderInfo.Rows[0]["LineCode"]}',N'{workorderInfo.Rows[0]["LineName"]}',N'{workorder}',
N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',
N'{workorderInfo.Rows[0]["MaterialTypeCode"]}',N'{workorderInfo.Rows[0]["MaterialTypeName"]}',N'{workorderInfo.Rows[0]["MaterialCode"]}',N'{workorderInfo.Rows[0]["MaterialName"]}',1,0,0,0,N'{lotcode}',0,4373791262185230336,GETDATE());";

                if (!flag)
                {
                    var tmp = checkOldmes(oldDb, code);
                    if (tmp)
                    {
                        Console.WriteLine("同步成功：" + code);
                        res = oldDb.Ado.ExecuteCommand(inLineSql);
                    }
                }
            }
            else if (name.Contains("前梁"))
            {
                // 获取工单信息
                liaohao = "21600000006";
                routeCode = "21601000006";
                zrouteCode = "T1BA10002";
                workorderSelect = @$"select 
'21601000006' MaterialCode,a.name MaterialName , a.WONo,rou.ProductionLineCode LineCode,rou.ProductionLineName LineName,rou.RouteCode,
p.ProcessCode,p.ProcessIdentify,p.ProcessName,p.BackProcessCode,p.BackProcessIdentify,p.BackProcessName,
rou.MaterialTypeCode,rou.MaterialTypeName 
from (
	select wo.MaterialCode,MaterialName, wo.WONo,bm.MaterialTypeCode,bm.MaterialTypeName,bm.code,bm.name  from 
	WorkSchedule_WO  wo , Basic_MaterialCategory bm
	where wo.WONo = '{workorder}' and bm.Code  = '{liaohao}' 
	and wo.WOStatus  = '生产中' and wo.IsDeleted  = 0 
) a  left join WorkOrder_Route rou on rou.WONo  = a.WONo and a.MaterialTypeCode = rou.MaterialTypeCode and rou.RouteCode = '{routeCode}'
left join WorkOrder_RouteProcess p on p.WONo  = a.WONo and p.RouteCode  = rou.RouteCode 
where   p.IsFirstProcess = 1 and p.ProcessSequence = 1";

                var workorderInfo = oldDb.Ado.GetDataTable(workorderSelect);

                if (workorderInfo.Rows.Count != 1)
                {
                    Console.Write($"工单信息[{workorder}]有误，共查询到【 {workorderInfo.Rows.Count} 】条数据");
                    return;
                }

                // 获取mes唯一数字码
                sequenceCode = getSequenceId(Db);
                inLineSql = $@"INSERT INTO DataCollection_MainStatus (Id,BarCode,NumberTypeCode,NumberTypeName,
ContainerLayer,ContainerRow,ContainerColumn,
QualityStatus,RouteCode,LineCode,LineName,WONo,
LastProcessCode,LastProcessIdentify,LastProcessName,NextProcessCode,NextProcessIdentify,NextProcessName,
MaterialTypeCode,MaterialTypeName,MaterialCode,MaterialName,MaterialCount,IsFinishProcess,IsConsumed,IsLock,BatchNo,IsDeleted,CreaterId ,CreateTime) 
VALUES({sequenceCode},N'{code}',N'BatchCode',N'批次号',
0,0,0,N'OK',N'{workorderInfo.Rows[0]["RouteCode"]}',N'{workorderInfo.Rows[0]["LineCode"]}',N'{workorderInfo.Rows[0]["LineName"]}',N'{workorder}',
N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',N'{workorderInfo.Rows[0]["BackProcessCode"]}',N'{workorderInfo.Rows[0]["BackProcessIdentify"]}',N'{workorderInfo.Rows[0]["BackProcessName"]}',
N'{workorderInfo.Rows[0]["MaterialTypeCode"]}',N'{workorderInfo.Rows[0]["MaterialTypeName"]}',N'{workorderInfo.Rows[0]["MaterialCode"]}',N'{workorderInfo.Rows[0]["MaterialName"]}',1,0,0,0,N'{lotcode}',0,4373791262185230336,GETDATE());";
                
                if(flag)
                {
                    var tmp = checkOldmes(oldDb, code);
                    if (tmp)
                    {
                        Console.WriteLine("同步成功：" + code);
                        res = oldDb.Ado.ExecuteCommand(inLineSql);
                    }
                    continue;
                }


                // 获取mes唯一数字码 总成码导入
                sequenceCode = getSequenceId(Db);
                workorderSelect = @$"select 
a.MaterialCode ,a.MaterialName ,a.WONo,rou.ProductionLineCode LineCode,rou.ProductionLineName LineName,rou.RouteCode,
p.ProcessCode,p.ProcessIdentify,p.ProcessName,p.BackProcessCode,p.BackProcessIdentify,p.BackProcessName,
rou.MaterialTypeCode,rou.MaterialTypeName 
from (
	select wo.MaterialCode,MaterialName, wo.WONo,bm.MaterialTypeCode,bm.MaterialTypeName,bm.code,bm.name  from 
	WorkSchedule_WO  wo , Basic_MaterialCategory bm
	where wo.WONo = '{workorder}' and bm.Code  = '{liaohao}' 
	and wo.WOStatus  = '生产中' and wo.IsDeleted  = 0 
) a  left join WorkOrder_Route rou on rou.WONo  = a.WONo  and rou.RouteCode = '{zrouteCode}'
left join WorkOrder_RouteProcess p on p.WONo  = a.WONo and p.RouteCode  = rou.RouteCode 
where   p.IsFirstProcess = 1 and p.ProcessSequence = 1";
                workorderInfo = oldDb.Ado.GetDataTable(workorderSelect);

                if (workorderInfo.Rows.Count != 1)
                {
                    Console.Write($"工单信息[{workorder}]有误，共查询到【 {workorderInfo.Rows.Count} 】条数据");
                    return;
                }
                inLineSql = $@"INSERT INTO DataCollection_MainStatus (Id,BarCode,NumberTypeCode,NumberTypeName,
ContainerLayer,ContainerRow,ContainerColumn,
QualityStatus,RouteCode,LineCode,LineName,WONo,
LastProcessCode,LastProcessIdentify,LastProcessName,NextProcessCode,NextProcessIdentify,NextProcessName,
MaterialTypeCode,MaterialTypeName,MaterialCode,MaterialName,MaterialCount,IsFinishProcess,IsConsumed,IsLock,BatchNo,IsDeleted,CreaterId ,CreateTime) 
VALUES({sequenceCode},N'{code}',N'BatchCode',N'批次号',
0,0,0,N'OK',N'{workorderInfo.Rows[0]["RouteCode"]}',N'{workorderInfo.Rows[0]["LineCode"]}',N'{workorderInfo.Rows[0]["LineName"]}',N'{workorder}',
N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',N'{workorderInfo.Rows[0]["ProcessCode"]}',N'{workorderInfo.Rows[0]["ProcessIdentify"]}',N'{workorderInfo.Rows[0]["ProcessName"]}',
N'{workorderInfo.Rows[0]["MaterialTypeCode"]}',N'{workorderInfo.Rows[0]["MaterialTypeName"]}',N'{workorderInfo.Rows[0]["MaterialCode"]}',N'{workorderInfo.Rows[0]["MaterialName"]}',1,0,0,0,N'{lotcode}',0,4373791262185230336,GETDATE());";

                if(!flag)
                {
                    var tmp = checkOldmes(oldDb, code);
                    if (tmp)
                    {
                        Console.WriteLine("同步成功：" + code);
                        res = oldDb.Ado.ExecuteCommand(inLineSql);
                    }
                        
                }

            }


        }

    }

    // 获取mes唯一ID
    private static long getSequenceId(SqlSugarClient Db)
    {
        // 获取mes唯一序列号
        var sequenceSql = "select next value for BarcodeDetailRelation as sequenceCode";
        var sequenceObj = Db.Ado.GetDataTable(sequenceSql);
        long sequenceCode = Convert.ToInt64(sequenceObj.Rows[0][0]);
        return sequenceCode;
    }

    private static bool checkOldmes(SqlSugarClient oldDb,string code)
    {
        var checkInfo = oldDb.Ado.GetDataTable(
                $"select * from DataCollection_MainStatus where BarCode  = '{code}'");
        if (checkInfo.Rows.Count >= 1)
        {
            Console.Write("Y ");
            return false;
        }
        if (checkInfo.Rows.Count != 1)
        {
            Console.WriteLine("");
            Console.WriteLine(" 编码异常数量为"+ checkInfo.Rows.Count+ " 编码为：" + code);
        }
        return true;
    }
}*/
#endregion
#region 读取linux系统日志
/*internal class Program
{
    // 定义远程服务器的连接信息
    static string host = "10.11.26.22";
    static int port = 22; // 默认的SSH端口是22
    static string username = "superadmin";
    static string password = "Qwaszx123@#";
    static bool StopFlag = false;
    static void Main()
    {
        CreateLogThread();
        Thread.Sleep(100000);
        StopFlag = true;
        Console.WriteLine("睡眠结束");
        CreateLogThread();


    }

    private static void CreateLogThread()
    {
        Thread t = new Thread(listenLinux);
        t.Start();
    }

    private static void listenLinux()
    {
        // 创建SshClient实例
        using (var client = new SshClient(host, port, username, password))
        {
            try
            {
                // 尝试连接到服务器
                client.Connect();

                if (client.IsConnected)
                {
                    var cmd = client.CreateCommand($"tail -f ../../docker/net/MES/Logs/log20241108.txt");
                    var rasyncht = cmd.BeginExecute();

                    using (var reader = new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
                    {
                        while (!rasyncht.IsCompleted || !reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line != null)
                            {
                                Console.WriteLine(line);
                            }
                            if(StopFlag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 连接失败的异常处理
                Console.WriteLine($"Unable to establish SSH connection: {ex.Message}");
            }
            finally
            {
                // 总是确保在结束时断开连接
                if (client.IsConnected)
                {
                    client.Disconnect();
                    Console.WriteLine("SSH connection disconnected.");
                }
            }
        }
    }

}*/
#endregion
#region sqlsugar链接
//internal class Program
//{
//    static void Main(string[] args)
//    {

//        SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
//        {
//            ConnectionString = "Data Source=10.11.26.220;Initial Catalog=MES;Persist Security Info=True;User ID=sa;Password=Knt@654321;Connect Timeout=500;Encrypt=false;",
//            DbType = DbType.SqlServer,
//            IsAutoCloseConnection = true
//        });
//        var dt = Db.Ado.GetDataTable("select * from Basic_BarcodeGenerate where IsEnable = 1");
//        Console.WriteLine(dt);
//    }
//}
#endregion
#region 文件路径拆解
/*public class program
{
    static void Main(string[] args)
    {
        // 获取当前运行程序的目录
        string fileDir = Environment.CurrentDirectory;
        Console.WriteLine("当前程序目录：" + fileDir);

        // 一个文件目录
        string filePath = "C:\\JiYF\\BenXH\\BenXHCMS.xml";
        Console.WriteLine("该文件的目录：" + filePath);

        // 获取文件的全路径
        string str = "获取文件的全路径：" + Path.GetFullPath(filePath);
        Console.WriteLine(str);

        // 获取文件所在的目录
        string strDir = "获取文件所在的目录：" + c;
        Console.WriteLine(strDir);

        // 获取文件的名称含有后缀
        string strFileName = "获取文件的名称含有后缀：" + Path.GetFileName(filePath);
        Console.WriteLine(strFileName);

        // 获取文件的名称没有后缀
        string strFileNameNoExt = "获取文件的名称没有后缀：" + Path.GetFileNameWithoutExtension(filePath);
        Console.WriteLine(strFileNameNoExt);

        // 获取路径的后缀扩展名称
        string strExt = "获取路径的后缀扩展名称：" + Path.GetExtension(filePath);
        Console.WriteLine(strExt);

        // 获取路径的根目录
        string strRootPath = "获取路径的根目录：" + Path.GetPathRoot(filePath);
        Console.WriteLine(strRootPath);

        Console.ReadKey();
    }
}*/
#endregion
#region 获取IPV4地址
/*public class program
{
    public static void Main(string[] args)
    {
        try
        {
            IPAddress[] ipv4Addresses = GetLocalIPv4Addresses();
            Console.WriteLine("本机的IPv4地址：");
            foreach (IPAddress address in ipv4Addresses)
            {
                Console.WriteLine(address.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("获取IP地址时发生错误: " + ex.Message);
        }
    }
    static IPAddress[] GetLocalIPv4Addresses()
    {
        return NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
            .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(ua => ua.Address)
            .ToArray();
    }
}*/
#endregion
#region 定时器
/*class Scheduler
{
    private Timer timer;

    public Scheduler()
    {
        // 计算当前时间到下一个小时的55分之间的时间间隔
        DateTime now = DateTime.Now;
        TimeSpan timeToNextReminder = new TimeSpan(1, 0, 0) - TimeSpan.FromSeconds(now.Minute * 60 + now.Second) - TimeSpan.FromMinutes(55);

        // 创建定时器，设置回调函数和时间间隔
        timer = new Timer(Alert, null, timeToNextReminder, TimeSpan.FromHours(1));
    }

    private void Alert(object state)
    {
        // 在每小时的55分进行提醒
        DateTime now = DateTime.Now;
        if (now.Minute == 55)
        {
            Console.WriteLine("提醒：每小时的55分");
            // 在这里编写相应的提醒逻辑或调用其它方法
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Scheduler scheduler = new Scheduler();

        // 保持主线程运行，避免程序退出
        Console.ReadLine();
    }
}*/
#endregion
#region datatable 排序
/*public class program
{
    public static void Main(string[] args)
    {
        DataTable dataTable = new DataTable();

        // 假设order列的数据类型为整数，stepname列的数据类型为字符串，createdate列的数据类型为日期时间
        dataTable.Columns.Add("order", typeof(int));
        dataTable.Columns.Add("stepname", typeof(string));
        dataTable.Columns.Add("createdate", typeof(DateTime));

        // 添加数据到dataTable...
        dataTable.Rows.Add(1, "StepB", DateTime.Now.AddHours(-1));
        dataTable.Rows.Add(1, "StepB", DateTime.Now);
        dataTable.Rows.Add(1, "StepA", DateTime.Now);
        dataTable.Rows.Add(3, "StepC", DateTime.Now);
        dataTable.Rows.Add(2, "StepB", DateTime.Now);

        // 自定义的Comparer，用于对stepname列进行特定顺序排序
        

        // 排序规则：order列升序，stepname列按照指定数组顺序排序，createdate列降序
        string[] arr = { "StepB", "StepA", "StepC"  };
        DataTable sortedTable = dataTable.AsEnumerable()
        .OrderBy(row => row.Field<int>("order"))

        .ThenBy(row => Array.IndexOf(arr, row.Field<string>("stepname")))

        .ThenByDescending(row => row.Field<DateTime>("createdate"))

        .CopyToDataTable();
        Console.WriteLine(sortedTable.Rows.Count);
    }
}*/
#endregion
#region 异步执行process
/*public class Program
{
    static Process process;
    static CancellationTokenSource tokenSource;

    static void Main(string[] args)
    {
        tokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            tokenSource.Cancel();
        };

        Task.Run(async () => await StartProcessAsync(tokenSource.Token));

        while (true)
        {
            Console.WriteLine("Main thread is running...");
            Thread.Sleep(1000);
        }
    }

    static async Task StartProcessAsync(CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = "-jar C:\\Users\\12850950\\Desktop\\jar\\sparkExecuter-1.0.0.jar",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
        process.Exited += (s, e) =>
        {
            Console.WriteLine("The process exited with code " + process.ExitCode);
            // automatically restart the process if it exits unexpectedly
            if (!cancellationToken.IsCancellationRequested && process.ExitCode != 0)
            {
                Console.WriteLine("Restarting the process...");
                Task.Run(async () => await StartProcessAsync(cancellationToken));
            }
        };

        try
        {
            Console.WriteLine("Starting the process...");
            process.Start();
            var reader = process.StandardOutput;
            while (!reader.EndOfStream)
            {
                Console.WriteLine(reader.ReadLine());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error starting the process: " + ex.Message);
            tokenSource.Cancel();
        }

        await Task.CompletedTask;
    }
}*/
#endregion
#region c# 调用java接口下载csv文件
/*public class program
{
    public static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var requestUri = "your_url";
        var content = new StringContent(JsonConvert.SerializeObject(new
        {
            // input参数是您自定义的参数，根据您的要求添加至请求体中
            // ...
        }), Encoding.UTF8, "application/json");

        var responseStream = await(await httpClient.PostAsync(requestUri, content)).Content.ReadAsStreamAsync();

        using (var fileStream = new FileStream("result.csv", FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, len);
            }
        }
    }
}*/
#endregion
#region roc 系统 代码生成sqlserver
/*public class program
{
    public static void Main(string[] args)
    {
        string DataBase = "roc";
        // 要生成的表
        string tableName = "xxxx";
        // 实体类是否需要创建人信息
        bool createFlag = true;
        // 要跳过的字段
        // string[] skipArr = new string[] { "ID", "CREATEDATE", "CREATEUSERID", "CREATEUSERNAME", "MODIFYDATE", "MODIFYUSERID", "MODIFYUSERNAME", "REMARK", "ENABLED" };
        string[] skipArr = new string[] { "ID" };
        if (createFlag)
            skipArr = new string[] { "ID", "CREATEDATE", "CREATEUSERID", "CREATEUSERNAME", "ENABLED" };
        // 版本号
        string version = "1.6";
        // 文件生成地址(默认不传就是桌面)
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/generate/"+ tableName;
        // 要显示的作者
        string author = "TP";
        // 需要显示的名称空间
        string nameSpaceStr = "Single";
        // SQLSERVER 
        var sqlserverConn1 = $"Server=192.168.0.00;DataBase={DataBase};User ID=sa;Pwd=000000;connection timeout=600";

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        // 1，连接数据库
        SqlSugarClient client = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = sqlserverConn1,
            DbType = SqlSugar.DbType.SqlServer,
            IsAutoCloseConnection = true
        });

        // 2,查询表字段
        string sql = $@"select b.name tablename,a.name columnname,a.name columnComment,c.name columnType
　　                   from {DataBase}..syscolumns a 
　　                   INNER JOIN {DataBase}..sysobjects b on  a.id=b.id 
　　                    left join {DataBase}..systypes c on a.xtype = c.xtype
　　                    where B.name='{tableName}'";
        var dt = client.Ado.GetDataTable(sql);

        // 3、开始生成实体
        string entityStr = File.ReadAllText("Template/Entity.txt");
        entityStr = entityStr.Replace("{inheritEntity}", createFlag ? "RocSupplyEntity" : "RocEntity");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{tableName}", tableName);
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName));
        // 开始填充实体类字段
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"]?.ToString()?.ToUpper()))
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
            sb.Append(ToPascal(dt.Rows[i][1]?.ToString() ?? ""));
            sb.Append(' ');
            sb.Append(" { set; get;} ");
        }
        entityStr = entityStr.Replace("{content}", sb.ToString());

        var entityPath = filePath + "/" + ToPascal(tableName) + ".cs";
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
        File.WriteAllText(controllerPath, entityStr.ToString());
        // 5、开始生成 IService
        entityStr = File.ReadAllText("Template/IService.txt");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName));
        var IServicePath = filePath + "/I" + ToPascal(tableName) + "Service.cs";
        File.WriteAllText(IServicePath, entityStr.ToString());
        // 6、开始生成 Service
        entityStr = File.ReadAllText("Template/Service.txt");
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"].ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ID")
                continue;
            sb.Append('\n');
            sb.Append("            ");
            sb.Append(".WhereIF(");
            sb.Append('\n');
            sb.Append("                ");
            var field = ToPascal(dt.Rows[i]["columnName"]?.ToString() ?? "");
            sb.Append(checkTypeFlag(checkType(dt.Rows[i][3].ToString()), field));
            sb.Append('\n');
            sb.Append("                ");
            sb.Append(checkTypeExpression(checkType(dt.Rows[i][3].ToString()), field));
        }
        entityStr = entityStr.Replace("{queryContent}", sb.ToString());
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{DataBase}", DataBase);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName));
        var ServicePath = filePath + "/" + ToPascal(tableName) + "Service.cs";
        File.WriteAllText(ServicePath, entityStr.ToString());
        // 7、开始生成 pageEntity
        entityStr = File.ReadAllText("Template/RocPage.txt");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        var pageEntityPath = filePath + "/RocPage.cs";
        File.WriteAllText(pageEntityPath, entityStr.ToString());

        // 8 生成前端 api.js
        entityStr = File.ReadAllText("Template/UI/api.txt");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName).ToLower());
        var apiPath = filePath + "/" + ToPascal(tableName, true) + ".js";
        File.WriteAllText(apiPath, entityStr.ToString());

        // 9 生成 view.vue
        entityStr = File.ReadAllText("Template/UI/view.txt");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{jsName}", ToPascal(tableName, true) + ".js");


        //{searchInput} <el-input v-model="listQuery.title" :placeholder="$t('pleaseEnter') + $t('title')" style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
        // 替换{searchInput}  **{listQuery}
        var listQuery = "";
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"].ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ENABLED")
                continue;
            if (dt.Rows[i]["columnName"]?.ToString().ToLower().IndexOf("name") != -1 || dt.Rows[i]["columnName"]?.ToString().ToLower().IndexOf("title") != -1)
            {
                var field = ToPascal(dt.Rows[i]["columnName"]?.ToString() ?? "", true);
                sb.Append("<el-input v-model=\"listQuery.");
                sb.Append(field);
                sb.Append("\" :placeholder=\"$t('pleaseEnter') + $t('");
                sb.Append(field);
                sb.Append("')\" style=\"width: 200px;\" class=\"filter-item\" @keyup.enter.native=\"handleFilter\" />");
                sb.Append("\r\n\t\t\t");

                listQuery += field;
                listQuery += " : '',";
            }
        }
        entityStr = entityStr.Replace("{queryParam}", listQuery.Substring(0, listQuery.Length - 1).Replace(": ''",""));
        entityStr = entityStr.Replace("{listQuery}", listQuery.Substring(0, listQuery.Length - 1));
        entityStr = entityStr.Replace("{searchInput}", sb.ToString());

        // 替换 {content} 
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"].ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ENABLED")
                continue;

            var field = ToPascal(dt.Rows[i]["columnName"]?.ToString() ?? "", true);
            if (field == "enabled")
            {
                sb.Append(@"<el-table-column :label=""$t('enabled')"" class-name=""status-col"" width=""100"">
				<template slot-scope=""{row}"">
					<el-tag :type=""row.enabled | statusFilter"">
						{{ row.enabled }}
					</el-tag>
				</template>
			</el-table-column>");
            }
            else
            {
                sb.Append("<el-table-column :label=\"$t('");
                sb.Append(field);
                sb.Append(@"')"" min-width=""140"" align=""center"">
				<template slot-scope=""{row}"">
					<span>{{ row.");
                sb.Append(field);
                sb.Append(@" }}</span>
				</template>
			</el-table-column>");
            }

            sb.Append("\r\n\t\t\t");

        }
        entityStr = entityStr.Replace("{content}", sb.ToString());

        // 替换{dialog}
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"].ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ENABLED")
                continue;
            var field = ToPascal(dt.Rows[i]["columnName"]?.ToString() ?? "", true);
            var fieldType = checkType(dt.Rows[i][3].ToString());
            sb.Append(@"<el-form-item :label=""$t('");
            sb.Append(field);
            sb.Append(@"')"" prop=""category"">
					<el-input v-model=""submitData.");
            sb.Append(field);
            sb.Append(@""" type=""");

            switch (fieldType)
            {
                case "string?":
                    sb.Append("text");
                    break;
                case "date?":
                    sb.Append("date");
                    break;
                case "int?":
                case "decimal":
                case "long?":
                    sb.Append("number");
                    break;
                default:
                    sb.Append("text");
                    break;
            }
            sb.Append(@""" />
				</el-form-item>");
            sb.Append("\r\n\t\t\t\t");
        }
        entityStr = entityStr.Replace("{dialog}", sb.ToString());



        var viewPath = filePath + "/" + ToPascal(tableName, true) + ".vue";
        File.WriteAllText(viewPath, entityStr.ToString());

    }

    private static string checkTypeExpression(string type, string field)
    {
        string compareExpression;
        switch (type)
        {
            case "string?":
            case "string":
                if (field.ToLower().IndexOf("name") != -1 || field.ToLower().IndexOf("title") != -1)
                    compareExpression = $"x => x.{field}.Contains(input.{field}))";
                else
                    compareExpression = $"x => x.{field} == input.{field} )";
                break;
            case "int?":
            case "int":
            case "long?":
            case "long":
            case "decimal?":
            case "decimal":
            case "Date?":
            case "Date":
                compareExpression = $"x => x.{field} == input.{field} )";
                break;
            default:
                compareExpression = $"x => x.{field} == input.{field} )";
                break;
        }
        return compareExpression;
    }

    private static string checkTypeFlag(string type, string field)
    {
        string flagExpression;
        switch (type)
        {
            case "string?":
            case "string":
                flagExpression = $"input.{field}.IsNotNullOrEmpty() ,";
                break;
            case "int?":
            case "int":
            case "long?":
            case "long":
            case "decimal?":
            case "decimal":
            case "Date?":
            case "Date":
                flagExpression = $"input.{field} != null ,";
                break;
            default:
                flagExpression = $"input.{field}.IsNotNullOrEmpty() ,";
                break;
        }
        return flagExpression;
    }

    private static string ToPascal(string str, bool firstLow = false)
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

        switch (newStr)
        {
            case "Arrivalstatus":
                newStr = "ArrivalStatus";
                break;
            case "BoomId":
                newStr = "BoomId";
                break;
            case "CreateuserId":
                newStr = "CreateuserId";
                break;
            case "CreateuserName":
                newStr = "CreateuserName";
                break;
            case "CreateDate":
                newStr = "CreateDate";
                break;
            case "Freeinspection":
                newStr = "FreeInspection";
                break;
            case "Itemcount":
                newStr = "ItemCount";
                break;
            case "Itemcode":
                newStr = "ItemCode";
                break;
            case "Menubuttonid":
                newStr = "MenubuttonId";
                break;
            case "Roleid":
                newStr = "RoleId";
                break;
            case "Parentid":
                newStr = "ParentId";
                break;
            case "Parentcount":
                newStr = "ParentCount";
                break;
            case "Partname":
                newStr = "PartName";
                break;
            case "Productname":
                newStr = "ProductName";
                break;
            case "Pickingstatus":
                newStr = "PickingStatus";
                break;
            case "Qualitystatus":
                newStr = "QualityStatus";
                break;
            case "Rolename":
                newStr = "RoleName";
                break;
            case "Sortcode":
                newStr = "SortCode";
                break;
            case "Specialpartname":
                newStr = "SpecialPartname";
                break;

        }
        if (firstLow)
        {
            char[] chars = newStr.ToCharArray();
            chars[0] = char.ToLower(chars[0]);
            newStr = new string(chars);
            return newStr;
        }
        return newStr;
    }

    private static string checkType(string? value)
    {
        switch (value ?? "VARCHAR2".ToUpper())
        {
            case "VARCHAR2":
            case "VARCHAR":
            case "CHAR":
            case "TEXT":
            case "CLOB":
                return "string?";
            case "DATE":
            case "TIME":
            case "DATETIME":
            case "TIMESTAMP":
                return "Date?";
            case "DECIMAL":
            case "DOUBLE PRECISION":
            case "NUMBER":
                return "decimal";
            case "INTEGER":
            case "int":
                return "int?";
            case "LONG":
                return "long?";
            default:
                return "string?";
        }
    }
}*/
#endregion
#region 代码生成oracle
/*public class program
{
    public static void Main(string[] args)
    {
        // 要生成的表
        string tableName = "BI_COLLECT";
        // 要跳过的字段
        string[] skipArr = new string[] { "ID", "CREATEDATE", "CREATEUSERID", "CREATEUSERNAME", "MODIFYDATE", "MODIFYUSERID", "MODIFYUSERNAME", "REMARK", "ENABLED" };
        // 版本号
        string version = "1.1";
        // 文件生成地址(默认不传就是桌面)
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/generate";
        // 要显示的作者
        string author = "GPF";
        // 需要显示的名称空间
        string nameSpaceStr = "Bi";
        //锦溪正式Oracle
        var oracleConn1 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;";

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
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"]?.ToString()?.ToUpper()))
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
            sb.Append(ToPascal(dt.Rows[i][1]?.ToString() ?? ""));
            sb.Append(' ');
            sb.Append(" { set; get;} ");
        }
        entityStr = entityStr.Replace("{content}", sb.ToString());

        var entityPath = filePath + "/" + ToPascal(tableName) + ".cs";
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
        File.WriteAllText(IServicePath, entityStr.ToString());
        // 6、开始生成 Service
        entityStr = File.ReadAllText("Template/Service.txt");
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"].ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ID")
                continue;
            sb.Append('\n');
            sb.Append("            ");
            sb.Append(".WhereIF(");
            sb.Append('\n');
            sb.Append("                ");
            var field = ToPascal(dt.Rows[i]["columnName"]?.ToString() ?? "");
            sb.Append($"!string.IsNullOrEmpty(input.{field}),");
            sb.Append('\n');
            sb.Append("                ");
            sb.Append($"x => x.{field}.Contains(input.{field}))");
        }
        entityStr = entityStr.Replace("{queryContent}", sb.ToString());
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName));
        var ServicePath = filePath + "/" + ToPascal(tableName) + "Services.cs";
        File.WriteAllText(ServicePath, entityStr.ToString());
        // 6、开始生成 Input
        entityStr = File.ReadAllText("Template/Input.txt");
        sb.Clear();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Columns.Contains("columnName") && skipArr.Contains(dt.Rows[i]["columnName"]?.ToString()?.ToUpper()) && dt.Rows[i]["columnName"].ToString()?.ToUpper() != "ID")
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
            sb.Append(ToPascal(dt.Rows[i][1]?.ToString() ?? ""));
            sb.Append(' ');
            sb.Append(" { set; get;} ");
        }
        entityStr = entityStr.Replace("{content}", sb.ToString());
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        entityStr = entityStr.Replace("{className}", ToPascal(tableName));
        var InputPath = filePath + "/" + ToPascal(tableName) + "Input.cs";
        File.WriteAllText(InputPath, entityStr.ToString());
        // 6、开始生成 pageEntity
        entityStr = File.ReadAllText("Template/PageEntity.txt");
        entityStr = entityStr.Replace("{namespace}", nameSpaceStr);
        entityStr = entityStr.Replace("{description}", " This is the class description");
        entityStr = entityStr.Replace("{auther}", author);
        entityStr = entityStr.Replace("{createDate}", DateTime.Now.ToLocalTime().ToString());
        entityStr = entityStr.Replace("{version}", version);
        var pageEntityPath = filePath + "/PageEntity.cs";
        File.WriteAllText(pageEntityPath, entityStr.ToString());
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
        switch (value ?? "VARCHAR2".ToUpper())
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
}*/
#endregion
#region c# 连接 starRock
/*public class program
{
    [Obsolete]
    public static void Main(string[] args)
    {
        string dns = "Server=10.32.44.207;User ID=root;Password=luxshare;port=9030;Database=default";
        SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = dns,
            DbType = SqlSugar.DbType.MySqlConnector,
            IsAutoCloseConnection = true,
        });
        Console.WriteLine("创建链接");
        var dt2 = Db.Ado.GetDataTable("select * from mc_model_hive_ro where modelname='LA4';");
        Console.WriteLine("获取数据成功: 数量为" + dt2.Rows.Count);

    }
}*/
#endregion
#region 连接sqllite
/*public class program
{
    public static void Main(string[] args)
    {
        string connect = "Data Source=C:\\Users\\pengfei_ge\\Desktop\\delete\\sqlite.db";
        SqlSugarClient client = new SqlSugarClient(new ConnectionConfig
        {
            ConfigId = "A",
            ConnectionString = connect,
            DbType = SqlSugar.DbType.Sqlite,
        });
        var dt = client.Ado.GetDataTable("select * from sys_user");
        Console.WriteLine(dt.Rows.Count);
    }
}*/
#endregion
#region 连接sqlserver
/*public class program
{
    public static void Main(string[] args)
    {
        string connect = "Server=192.168.0.00;DataBase=bireport;User ID=sa;Pwd=000000";
        SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
        {
            ConfigId = "A",
            ConnectionString = connect,
            DbType = SqlSugar.DbType.SqlServer
        });
        var dt = sqlSugarClient.Ado.GetDataTable("select * from  sys_dataitem");
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
        string connect = "Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=00.00.00.00;Port=8123;User=default;Password=;SocketTimeout=600000;Database=datasets;";
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
        var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;";
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
        var oracleConn1 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;";
        // 越南正式oracle
        var oracleConn2 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST =00.00.00.00)(PORT = 1521))(CONNECT_DATA=(SERVICE_NAME=test))) ;Persist Security Info=True;User ID=aa;Password=aa;";
        // 锦溪测试Oracle
        //var oracleConn2 = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;";
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
        var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST =00.00.00.00)(PORT = 1521))(CONNECT_DATA=(SERVICE_NAME=test))) ;Persist Security Info=True;User ID=aa;Password=aa;";
        // 锦溪测试Oracle
        //var oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;";
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
#region 数据库搬迁从sqlserver 到 postgresql
/*public class Program
{
    public static void Main(string[] args)
    {
        // 
        string tableName = "Equ";
        string schemaName = "dbo";
        bool createFlag = false;
        bool dataFlag = true;
        var sqlserverConn = "Server=10.11.26.220;Database=wms;User Id=sa;Password=Knt@654321;Encrypt=True;TrustServerCertificate=True";

        var pgsqlConn = "Host=10.11.22.235;Port=5432;Database=wms;Username=kntwms;Password=qwaszx123;";

        SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
        {
            ConfigId = "A",
            ConnectionString = sqlserverConn,
            DbType = SqlSugar.DbType.SqlServer
        });
        // 生成建表语句
        List<string> sqls = new();
        var tableSql = getTableInfoSql(DbType.SqlServer, tableName, schemaName);
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
            tableCreat.Append(getType(SqlSugar.DbType.SqlServer, SqlSugar.DbType.PostgreSQL, item));
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
        var sql = $"select * from {schemaName}.{tableName}";
        var dt = sqlSugarClient.Ado.GetDataTable(sql);
        sqlSugarClient.Close();
        sqlSugarClient.Dispose();
        // 链接Oracle
        SqlSugarClient sqlSugarClient2 = new SqlSugarClient(new ConnectionConfig
        {
            ConfigId = "A",
            ConnectionString = pgsqlConn,
            DbType = SqlSugar.DbType.Oracle
        });
        var dt2 = sqlSugarClient2.Ado.GetDataTable("select * from sysuser");
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
            sqlSugarClient2.Fastest<System.Data.DataTable>().AS(tableName.ToLower()).BulkCopy(dt);
        sqlSugarClient2.Close();
        sqlSugarClient2.Dispose();
    }

    private static string getTableInfoSql(DbType type, string tableName, string schemaName)
    {
        string sql = "";
        switch (type)
        {
            case DbType.MySql:
                sql = $@"SELECT  distinct TABLE_NAME tableName
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
                            and TABLE_SCHEMA ='{schemaName}'";
                break;
            case DbType.SqlServer:
                sql = $@"SELECT DISTINCT 
                            t.name AS tableName,
                            c.name AS columnName,
                            CASE 
                                WHEN pk.column_id IS NOT NULL THEN 'PRIMARY KEY'
                                ELSE ''
                            END AS columnConstraint,
                            ep.value AS columnComment,
                            ty.name AS columnType,
                            c.max_length AS columnLength,
                            c.precision AS columnNumberPrecision,
                            c.scale AS columnNumberScale,
                            c.default_object_id AS columnDefaultObjectId, -- 默认值对象ID
                            OBJECT_DEFINITION(c.default_object_id) AS columnDefault, -- 默认值表达式
                            c.is_nullable AS columnIsNullable
                        FROM 
                            sys.columns c
                        INNER JOIN 
                            sys.tables t ON c.object_id = t.object_id
                        INNER JOIN 
                            sys.schemas s ON t.schema_id = s.schema_id
                        INNER JOIN 
                            sys.types ty ON c.user_type_id = ty.user_type_id
                        LEFT JOIN 
                            sys.extended_properties ep ON c.object_id = ep.major_id AND c.column_id = ep.minor_id AND ep.class = 1
                        LEFT JOIN 
                            (SELECT ic.object_id, ic.column_id
                             FROM sys.indexes i
                             INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                             WHERE i.is_primary_key = 1) pk ON c.object_id = pk.object_id AND c.column_id = pk.column_id
                        WHERE 
                            t.name = '{tableName}' -- 替换为目标表名
                            AND s.name = '{schemaName}' -- 替换为目标模式名（Schema）";
                break;

        }
        return sql;
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
        string res = "VARCHAR(255)";
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
            case (SqlSugar.DbType.SqlServer, SqlSugar.DbType.PostgreSQL):
                switch (info.columnType)
                {
                    case "varchar":
                        if (info.columnLength == -1)
                            res = "text";
                        else
                            res = $"varchar({Math.Min(info.columnLength, 4000)})";
                        break;
                    case "int":
                        res = "int4";
                        break;
                    case "bigint":
                        res = "int8";
                        break;
                    case "datetime":
                        res = "timestamp";
                        break;
                    default:
                        res = $"text";
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
        var conn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=00.00.00.00)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=test)));Persist Security Info=True;User ID=aa;Password=aa;connect timeout = 600;";
        //var conn = "Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 00.00.00.00)(PORT = 1521))) (CONNECT_DATA =(SERVICE_NAME = test))); Persist Security Info=True;User ID=aa;Password=aa;connect timeout = 600;";
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
