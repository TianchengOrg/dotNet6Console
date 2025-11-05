using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testorm.Entity;

public class ProductInfo
{
    public string? id { get; set; }
    public string? barCode { get; set; }
    public string? numberTypeCode { get; set; }
    public string? numberTypeName { get; set; }
    public string? containerTypeCode { get; set; }
    public string? containerTypeName { get; set; }
    public string? containerCode { get; set; }
    public int? containerLayer { get; set; }
    public int? containerRow { get; set; }
    public int? containerColumn { get; set; }
    public string? materialCarCode { get; set; }
    public string? materialCarPosition { get; set; }
    public string? qualityStatus { get; set; }
    public string? routeCode { get; set; }
    public string? lineCode { get; set; }
    public string? lineName { get; set; }
    public string? woNo { get; set; }
    public string? startProcessCode { get; set; }
    public string? startProcessName { get; set; }
    public string? currentProcessCode { get; set; }
    public string? currentProcessName { get; set; }
    public string? currentProcessIdentify { get; set; }
    public string? agvProcessCode { get; set; }
    public string? agvProcessName { get; set; }
    public string? startTime { get; set; }
    public string? endTime { get; set; }
    public string? lastProcessCode { get; set; }
    public string? lastProcessName { get; set; }
    public string? nextProcessCode { get; set; }
    public string? nextProcessName { get; set; }
    public string? startProcessIdentify { get; set; }
    public string? lastProcessIdentify { get; set; }
    public string? nextProcessIdentify { get; set; }
    public string? materialTypeCode { get; set; }
    public string? materialTypeName { get; set; }
    public string? materialCode { get; set; }
    public string? materialName { get; set; }
    public int? materialCount { get; set; }
    public bool isFinishProcess { get; set; }
    public bool isConsumed { get; set; }
    public bool isLock { get; set; }
    public bool isMark { get; set; }
    public string? batchNo { get; set; }
    public string? createUserName { get; set; }
    public string? createRealName { get; set; }
    public string? modifierUserName { get; set; }
    public string? modifierRealName { get; set; }
    public string? createrId { get; set; }
    public string? createTime { get; set; }
    public string? modifierId { get; set; }
    public string? modifyTime { get; set; }
}

