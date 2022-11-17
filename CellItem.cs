using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm
{
    public class CellItem
    {
        /// <summary>
        /// 数据集编码  （禁止修改）
        /// </summary>
        public string setCode { get; set; }
        /// <summary>
        /// 数据集中的key，拖拽过来的值（禁止修改）
        /// </summary>
        public string setKey { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public int row { get; set; } = -1;
        /// <summary>
        /// 列
        /// </summary>
        public int column { get; set; } = -1;
        /// <summary>
        /// 行合并
        /// </summary>
        public int rowMerge { get; set; } = -1;
        /// <summary>
        /// 列合并
        /// </summary>
        public int columnMerge { get; set; } = -1;
        /// <summary>
        /// 原始行（禁止修改）
        /// </summary>
        public int coordinateRow { get; set; } = -1;
        /// <summary>
        /// 原始列（禁止修改）
        /// </summary>
        public int coordinateColumn { get; set; } = -1;
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 值类型（禁止修改）
        /// </summary>
        public string valueType { get; set; } = "string";


    }
}
