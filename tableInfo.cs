using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm
{
    public class tableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string columnName { get; set; }
        /// <summary>
        /// 字段约束（是否唯一，主键）
        /// </summary>
        public string columnConstraint { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string columnComment { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string columnType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int columnLength { get; set; }
        /// <summary>
        /// 字段八进制长度
        /// </summary>
        public int columnOctetLength { get; set; }
        /// <summary>
        /// 数字整数位
        /// </summary>
        public int columnNumberPrecision { get; set; }
        /// <summary>
        /// 数字小数位
        /// </summary>
        public int columnNumberScale { get; set; }
        /// <summary>
        /// 字段默认值
        /// </summary>
        public string columnDefault { get; set; }
        /// <summary>
        /// 字段是否为空
        /// </summary>
        public string columnIsNullable { get; set; }
    }
}
