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

namespace testorm
{
    /*public class program
    {
        public static void Main(string[] args)
        {
            
        }
    }*/
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
    #region 尝试调用OA系统消息 失败
    /*public class program
    {
        public static async Task Main(string[] args)
        {
            string appid = "F557F36E725A20134D084500D44407EA";
            string cpk;
            string pricpk;

            RSA rsa = RSA.Create(2048);
            
            // 获取公钥并转换为Base64字符串，这与Java中的处理方式可能不同
            byte[] publicKeyBytes = rsa.ExportRSAPublicKey();
            byte[] privateKeyBytes =  rsa.ExportRSAPrivateKey();
            string base64PublicKey = Convert.ToBase64String(publicKeyBytes);
            string private64KeyBytes = Convert.ToBase64String(privateKeyBytes);
            cpk =  base64PublicKey;
            pricpk = private64KeyBytes;
            

            // 创建HttpClient实例
            using var httpClient = new HttpClient();

            // 设置请求的URL
            string url = "http://192.168.1.22:8090/api/ec/dev/auth/regist";

            // 创建HttpRequestMessage，以便添加自定义Header
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // 添加Header信息
            request.Headers.Add("appid", appid);
            request.Headers.Add("cpk", cpk);

            // 发送POST请求
            HttpResponseMessage response = await httpClient.SendAsync(request);

            Result res;
            // 检查响应状态码
            if (response.IsSuccessStatusCode)
            {
                // 读取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                res = JsonSerializer.Deserialize<Result>(responseBody);
                Console.WriteLine($"Response: {responseBody}");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return;
            }
            // 获取token
            url = "http://192.168.1.22:8090/api/ec/dev/auth/applytoken";
            request = new HttpRequestMessage(HttpMethod.Post, url);


            //httpclient的使用请自己封装，可参考ECOLOGY中HttpManager类
            //HttpManager http = new HttpManager();
            //请求头信息封装集合
            //Map<String, String> heads = new HashMap<String, String>();
            //ECOLOGY返回的系统公钥

            String spk = res.spk;
            string secret = res.secret;
            string encryptedMessage;

            var bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(secret), RSAEncryptionPadding.OaepSHA1);
            encryptedMessage = Convert.ToBase64String(bytes);
            request.Headers.Add("appid", appid);
            request.Headers.Add("secret", encryptedMessage);
            response = await httpClient.SendAsync(request);

            // 检查响应状态码
            if (response.IsSuccessStatusCode)
            {
                // 读取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseBody}");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return;
            }


            //RSA rsa = new RSA();
            //对秘钥进行加密传输，防止篡改数据
            //String secret = rsa.encrypt(null, "调用注册接口返回的secret参数值", null, "utf-8", spk, false);
            //封装参数到请求头
            //heads.put("appid", "EEAA5436-7577-4BE0-8C6C-89E9D88805EA");
            //heads.put("secret", secret);
            //调用ECOLOGY系统接口进行申请
            //String data = http.postDataSSL("http://ip:port/api/ec/dev/auth/applytoken", null, heads);
            //返回的数据格式为json，具体格式参数格式请参考文末API介绍。
            //注意此时如果申请成功会返回token,请根据业务需要进行保存。
            //System.out.println(data);

        }
        public static RSAParameters ImportPublicKeyFromBase64(string base64PublicKey)
        {
            // 将Base64编码的公钥字符串解码为字节数组
            byte[] publicKeyBytes = Convert.FromBase64String(base64PublicKey);

            // 检查公钥前缀以确定其类型（例如，PKCS#1或PKCS#8）
            // 这里以PKCS#8为例，其开头通常是0x30 (SEQUENCE) followed by 0x81 (length)
            *//*if (publicKeyBytes[0] != 0x30 || publicKeyBytes[1] != 0x81)
            {
                throw new ArgumentException("The provided key is not in the expected PKCS#8 format.");
            }*//*

            // 移除PKCS#8包装，直接提取RSA公钥信息
            // 注意：此步骤取决于公钥确切的编码方式，可能需要调整
            int offset = 2; // PKCS#8头部长度简化示例，实际情况可能更复杂
            byte[] rsaPublicKeyBytes = new byte[publicKeyBytes.Length - offset];
            Array.Copy(publicKeyBytes, offset, rsaPublicKeyBytes, 0, rsaPublicKeyBytes.Length);

            // 使用RSA类来解析公钥字节并导出为RSAParameters
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(rsaPublicKeyBytes, out _);
                return rsa.ExportParameters(false); // 只导出公钥参数
            }
        }
    }*/
    #endregion
    #region 安防接口调用
    /*public class program
    {
        public static void Main(string[] args)
        {

            HttpUtillib.SetPlatformInfo("23211536", "QRGXyCbAAu34FYfc8vmc", "192.168.1.27", 1443, true);

            // 组装POST请求body   获取卡号信息       
            //string body = "{\"cardNo\": \"3151493508\"}";

            // 填充Url           
            //string uri = "/artemis/api/irds/v1/card/cardInfo";

            // 组装POST请求body    新增人员信息      
            //string body = "{\"PersonName\":\"葛饱饱\",\"Gender\":\"1\",\"OrgIndexCode\":\"476b1874-b49e-4a16-ab86-c5467456d008\",\"Birthday\":null,\"PhoneNo\":\"13000110011\",\"email\":\"person1@person.com\",\"certificateType\":\"111\",\"certificateNo\":\"11001001001100110011\",\"jobNo\":\"9999\",\"faces\":{\"faceData\":\"/9j/4AAQSkZJRgABAQEAAAAAAAD/4QBCRXhpZgAATU\"}}";

            // 填充Url           
            //string uri = "/artemis/api/resource/v2/person/single/add";

            // 组装POST请求body     新增人员信息     
            // string body = "[{\"clientId\": 1,\"PersonName\":\"葛饱饱\",\"Gender\":\"1\",\"OrgIndexCode\":\"476b1874-b49e-4a16-ab86-c5467456d008\",\"Birthday\":\"1990-01-01\",\"PhoneNo\":\"13000110011\",\"email\":\"person1@person.com\",\"certificateType\":\"111\",\"certificateNo\":\"11001001001100110011\",\"jobNo\":\"9999\"}]";

            // 填充Url           
            // string uri = "/artemis/api/resource/v1/person/batch/add";

            // 组装POST请求body       获取人员信息列表   
            //string body = "{\"pageNo\": 1,\"pageSize\": 2}";

            // 填充Url           
            //string uri = "/artemis/api/resource/v2/person/personList";
            // 组装POST请求body          获取门禁出入信息
            //string body = "{\"startTime\": \"2024-03-26T08:00:00.000+08:00\",\"endTime\": \"2024-03-26T08:30:00.000+08:00\"}";
            //var startTime = "2024-04-15T15:04:11+08:00";
            //var endTime = "2024-04-15T15:05:10+08:00";
            string body = $"{{\"pageNo\": 1,\"pageSize\": 1000,\"startTime\":\"2024-04-30T00:00:00.000+08:00\",\"endTime\":\"2024-08-01T00:00:00.000+08:00\",\"personName\":\"赵渴栋\"}}";
            //// 填充Url           
            string uri = "/artemis/api/acs/v2/door/events";

            // 组装POST请求body       获取部门信息列表   
            //string body = "{\"pageNo\": 1,\"pageSize\": 100}";

            // 填充Url           
            //string uri = "/artemis/api/resource/v1/org/orgList";

            // 发起POST请求，超时时间15秒，返回响应字节数组
            byte[] result = HttpUtillib.HttpPost(uri, body, 15);
            if (null == result)
            {
                // 请求失败
                Console.WriteLine("/artemis/api/irds/v1/card/cardInf: POST fail");
            }
            else
            {
                //// 注意：使用方应当知道哪个Url返回的是字节流（如根据图片Url获取图片数据流），
                ////       哪个肯定是字符串，如果肯定是返回字符串的，直接转成字符串即可
                ////       如果是返回字节流的，有可能因为请求失败了返回json报文，因此对于这种情况需要将字节流
                ////       转换成字符串判断是否存在失败的情况，如果存在失败则按字符串处理，否则认为返回的是字节流
                ////       本例中根据监控点编号查询监控点信息，不存在返回字节流的情况，直接转换成字符串来处理即可
                var json = System.Text.Encoding.UTF8.GetString(result);
                Console.WriteLine(json);
            }

            Console.WriteLine("调用");
        }
        private static string HmacSHA256(string secret, string signKey)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
                signRet = Convert.ToBase64String(hash);
                //signRet = ToHexString(hash); ;
            }
            return signRet;
        }

        private static string HmacSHA2562(string secret, string stringToSign)
        {
            string signRet = string.Empty;
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            HMACSHA256 mac = new HMACSHA256();
            mac.Key = keyBytes;
            byte[] signBytes = Encoding.UTF8.GetBytes(stringToSign);
            byte[] hash = mac.ComputeHash(signBytes);
            signRet = Convert.ToBase64String(hash);
            return signRet;
        }

        public static string HmacSHA256Encrypt(string secret, string signKey)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                mac.Initialize();
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(signKey));
                sbyte[] sb = new sbyte[hash.Length];
                for (int i = 0; i < hash.Length; i++)
                {
                    sb[i] = hash[i] <= 127 ? (sbyte)hash[i] : (sbyte)(hash[i] - 256);
                }
                byte[] unsignedByteArray = (byte[])(Array)sb;
                signRet = Convert.ToBase64String(unsignedByteArray);
            }
            return signRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public async static Task<string> Post()
        {
            DateTime date = DateTime.Now;
            var dateStr = date.ToString();
            var ak = "23211536";
            var sk = "QRGXyCbAAu34FYfc8vmc";
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateLong = Convert.ToInt64((date.ToUniversalTime() - epoch).TotalMilliseconds);
            var uri = "/artemis/api/v1/oauth/token";

            var signature = $"POST\nx-ca-key:{ak}\n{uri}";
            //var temp = "POST\n*\ntext/plain;charset=UTF-8\nheader-a:A\nheader-b:b\nx-ca-key:29666671\nx-ca-timestamp:1479968678000\n/artemis/api/example?a-body=a&qa=a&qb=B&x-body=x";

            Console.WriteLine(signature);

            var qianming = HmacSHA2562(sk, signature);

            Console.WriteLine(qianming);

            string result = "";
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("X-Ca-Key", ak);
            httpClient.DefaultRequestHeaders.Add("X-Ca-Signature", qianming);
            httpClient.DefaultRequestHeaders.Add("X-Ca-Signature-Headers", "x-ca-key");
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //httpClient.BaseAddress = new Uri("https://192.168.1.27:1443" + uri);

            var response = await httpClient.PostAsync("https://192.168.1.27:1443/artemis/api/v1/oauth/token", null);


            result = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(result);
            return result;
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
            string DataBase = "honghu";
            // 要生成的表
            string tableName = "mes_store_boom_items";
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
            var sqlserverConn1 = $"Server=192.168.1.25;DataBase={DataBase};User ID=sa;Pwd=Passw0rd;connection timeout=600";

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
            string connect = "Server=192.168.1.25;DataBase=bireport;User ID=sa;Pwd=Passw0rd";
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
