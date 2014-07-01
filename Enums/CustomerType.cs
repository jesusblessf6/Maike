using System.ComponentModel;

namespace Enums
{
    public enum CustomerType
    {
        [Description("客户公司")]
        Customer = 1,
        [Description("内部客户")]
        Internal = 2
    }
}
