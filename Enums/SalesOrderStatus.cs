using System.ComponentModel;

namespace Enums
{
    public enum SalesOrderStatus
    {
        [Description("订单已确定,打印合同")]
        OrderConfirmed = 1,
        [Description("合同已打印,收款")]
        ContractPrinted = 2,
        [Description("货款已收到,发货")]
        PaymentFinished = 3,
        [Description("已发货,完成订单")]
        DeliveryFinished = 4,
        [Description("订单已完成")]
        OrderFinished = 5,
        [Description("订单已取消")]
        OrderCancelled = 9
    }
}
