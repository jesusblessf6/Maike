using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// 该枚举同时用于定义合同的出具方
    /// </summary>
    public enum CompanyUserType
    {
        [Description("目前是买方，使用前台的公司和用户")]
        Counterparty = 1,
        [Description("目前是卖方，使用后台的公司和用户，例如迈科")]
        Self = 2
    }
}
