using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace testorm.Calculator;

public static class JsonFileHelper
{
    private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory;

    // 将对象序列化并保存到 JSON 文件
    public static void SaveToJson<T>(T obj,string fileName)
    {
        try
        {
            var realPath = Path.Combine(FilePath, fileName);
            string jsonString = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(realPath, jsonString);
            Console.WriteLine("数据已成功保存到 JSON 文件！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存 JSON 文件时出错：{ex.Message}");
        }
    }

    // 从 JSON 文件中读取并反序列化为对象
    public static T LoadFromJson<T>(string fileName)
    {
        var realPath = Path.Combine(FilePath, fileName);
        if (!File.Exists(realPath))
        {
            Console.WriteLine("JSON 文件不存在，返回默认值。");
            return default;
        }

        try
        {
            string jsonString = File.ReadAllText(realPath);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取 JSON 文件时出错：{ex.Message}");
            return default;
        }
    }

    // 清空 JSON 文件内容
    public static void ClearJsonFile(string fileName)
    {
        try
        {
            var realPath = Path.Combine(FilePath, fileName);
            File.WriteAllText(realPath, string.Empty);
            Console.WriteLine("JSON 文件内容已清空！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清空 JSON 文件时出错：{ex.Message}");
        }
    }
}
