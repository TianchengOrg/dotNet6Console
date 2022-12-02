using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm.Entity
{
    [SugarTable("auto_report")]
    public class Report 
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Column("reportName")]
        public String reportName { get; set; }


        /// <summary>
        /// 报表编码
        /// </summary>
        [Column("reportCode")]
        public String reportCode { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [Column("reportGroup")]
        public String reportGroup { get; set; }

        /// <summary>
        /// 报表描述
        /// </summary>
        [Column("reportDesc")]
        public String reportDesc { get; set; }

        /// <summary>
        /// 报表类型
        /// </summary>
        [Column("reportType")]
        public String reportType { get; set; }

        /// <summary>
        /// 报表缩略图
        /// </summary>
        [Column("reportImage")]
        public String reportImage { get; set; }

        /// <summary>
        /// 报表作者
        /// </summary>
        [Column("reportAuthor")]
        public String reportAuthor { get; set; }

        /// <summary>
        /// 下载次数
        /// </summary>
        [Column("downloadCount")]
        public int downloadCount { get; set; }

        /// <summary>
        /// 0--已禁用 1--已启用  DIC_NAME=ENABLE_FLAG
        /// </summary>
        [Column("enableFlag")]
        public int enableFlag { get; set; }

        /// <summary>
        /// 0--未删除 1--已删除 DIC_NAME=DELETE_FLAG
        /// </summary>
        [Column("deleteFlag")]
        public int deleteFlag { get; set; }
    }
}
